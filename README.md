# Observability Platform

A small .NET microservices observability platform built to test telemetry ingestion, endpoint metrics, SLA evaluation, alert generation, and load behavior.

The solution contains three API services:

- **Orders API**: simulated order endpoint.
- **Payments API**: simulated payment endpoint.
- **Telemetry Collector API**: receives telemetry events, aggregates endpoint metrics, calculates latency buckets/P95, checks SLA policies, and writes alerts.

It also includes an **NBomber console project** for load testing the Orders and Payments APIs.

## Architecture

```text
NBomber
   |
   | POST /api/orders
   | POST /api/payments
   v
+----------------+       telemetry event        +--------------------------+
| Orders API     | ---------------------------> | Telemetry Collector API  |
| Payments API   |                             | /api/telemetry           |
+----------------+                             +------------+-------------+
                                                            |
                                                            | in-memory channel
                                                            v
                                                +--------------------------+
                                                | TelemetryEventWorker     |
                                                | background consumer      |
                                                +------------+-------------+
                                                            |
                                                            v
                                                +--------------------------+
                                                | SQL Server               |
                                                | EndpointMetrics          |
                                                | EndpointLatencyBuckets   |
                                                | SlaPolicies              |
                                                | Alerts                   |
                                                +--------------------------+
```

## Projects

```text
src/
  BuildingBlocks/
    BuildingBlocks.Observability/
      Shared telemetry client, logging, correlation middleware, exception handling

  Services/
    OrdersService/
      Orders.Api/

    PaymentsService/
      Payments.API/

    TelemetryCollector/
      TelemetryCollector.Api/
      TelemetryCollector.Application/
      TelemetryCollector.Domain/
      TelemetryCollector.Infrastructure/

ConsoleApp1/
  NBomber load-test project
```

## Service URLs

When running with Docker Compose:

| Service | Swagger | Health |
|---|---|---|
| Orders API | `http://localhost:7000/swagger` | `http://localhost:7000/health` |
| Payments API | `http://localhost:7001/swagger` | `http://localhost:7001/health` |
| Telemetry Collector API | `http://localhost:7130/swagger` | Not currently mapped |

Inside Docker, the services communicate using container DNS names:

```text
Orders/Payments -> http://telemetry-api:8080
Telemetry health checks -> http://orders-api:8080 and http://payments-api:8080
```

## Run With Docker Compose

From the repository root:

```powershell
docker compose up -d --build
```

Check running containers:

```powershell
docker compose ps
```

View logs:

```powershell
docker compose logs -f
```

Stop the stack:

```powershell
docker compose down
```

The compose file builds and runs only the three API containers. It does not start SQL Server.

## Docker Compose Services

The root [docker-compose.yml](./docker-compose.yml) starts:

- `telemetry-api`
- `orders-api`
- `payments-api`

Orders and Payments depend on Telemetry because they send telemetry events to the collector.

```yaml
orders-api:
  depends_on:
    - telemetry-api

payments-api:
  depends_on:
    - telemetry-api
```

## SQL Server

The Telemetry Collector persists metrics and alerts to SQL Server.

The connection string is configured in:

```text
src/Services/TelemetryCollector/TelemetryCollector.Api/appsettings.Development.json
```

Default local development shape:

```json
"ConnectionStrings": {
  "TelemetryDb": "Server=localhost;Database=TelemetryCollectorDb;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

When running with Docker Compose, the telemetry connection string can be overridden from [docker-compose.yml](./docker-compose.yml) using the `ConnectionStrings__TelemetryDb` environment variable.

## Telemetry Flow

Orders and Payments simulate work, choose a status code, and send a telemetry event to the collector.

Each telemetry event contains:

- Service name
- Endpoint
- HTTP method
- Status code
- Response time in milliseconds
- Timestamp
- Correlation ID

The collector endpoint receives the event:

```text
POST /api/telemetry
```

It tries to enqueue the event into an in-memory channel:

```text
Queue has room -> return 202 Accepted
Queue is full  -> return 503 Service Unavailable
```

This prevents Orders and Payments from waiting indefinitely when the collector is overloaded.

## Background Processing

Telemetry events are processed by `TelemetryEventWorker`.

The worker consumes from the in-memory channel with parallel processing:

```text
MaxDegreeOfParallelism = 8
```

Each event is processed independently. If one event fails during processing, the worker logs the failure and continues processing the remaining events.

## Metrics

Metrics are aggregated in `EndpointMetrics`.

This table stores one row per:

```text
ServiceName + EndPoint + WindowStart
```

It does not store one row per request. Instead, every request updates counters on the aggregate row:

- `TotalRequests`
- `SuccessfulRequests`
- `ClientErrorRequests`
- `ServerErrorRequests`
- `TotalLatencyMs`

Example:

```text
30 NBomber scenarios/sec x 30 sec = 900 scenarios

Each scenario calls:
  1 Orders request
  1 Payments request

Expected metrics:
  Orders   TotalRequests = 900
  Payments TotalRequests = 900
```

## Latency Buckets And P95

Latency distribution is stored in `EndpointLatencyBuckets`.

This table is a histogram. It stores request counts grouped by latency bucket, for example:

```text
<= 50 ms
<= 100 ms
<= 250 ms
<= 500 ms
<= 1000 ms
...
```

If a request takes `187 ms`, it increments the `250 ms` bucket.

If a request takes `320 ms`, it increments the `500 ms` bucket.

The stored procedure uses these bucket counts to calculate an approximate P95 latency for each endpoint and time window.

Useful query:

```sql
SELECT
    ServiceName,
    EndPoint,
    WindowStart,
    BucketUpperBoundMs,
    RequestCount
FROM dbo.EndpointLatencyBuckets
ORDER BY WindowStart DESC, ServiceName, EndPoint, BucketUpperBoundMs;
```

To confirm bucket totals:

```sql
SELECT
    ServiceName,
    EndPoint,
    WindowStart,
    SUM(RequestCount) AS TotalBucketedRequests
FROM dbo.EndpointLatencyBuckets
GROUP BY ServiceName, EndPoint, WindowStart
ORDER BY WindowStart DESC;
```

## SLA Policies

SLA policies define thresholds for endpoints:

- Maximum error rate
- Maximum P95 latency
- Maximum consecutive health check failures

SLA endpoints:

```text
GET  /api/sla-policies
POST /api/sla-policies
GET  /api/sla-policies/{serviceName}/{endpoint}
```

## Alerts

The `Alerts` table stores SLA alert conditions, not every failed or slow request.

For example:

```text
Event:
  "Payment request returned HTTP 500"

Alert:
  "Payments /api/payments error rate is above the SLA threshold"
```

A failed request is still counted in `EndpointMetrics`.

A slow request is still counted in `EndpointLatencyBuckets`.

The `Alerts` table records the higher-level SLA condition.

## Alert Cooldown

The collector uses an in-memory alert cooldown store to prevent duplicate alert storms.

Cooldown key:

```text
ServiceName + EndPoint + AlertType
```

Default cooldown:

```json
"Alerting": {
  "CooldownSeconds": 60
}
```

During a 30-second load test, if Orders and Payments both violate latency and error-rate SLAs, the expected alert count is:

```text
Orders   HighLatency
Orders   HighErrorRate
Payments HighLatency
Payments HighErrorRate

Total: 4 alert rows
```

This is expected. It means four distinct SLA conditions occurred. It does not mean only four requests were slow or failed.

## Load Testing

The NBomber project is in:

```text
ConsoleApp1/
```

Run it with:

```powershell
dotnet run --project ConsoleApp1
```

The scenario calls both Orders and Payments.

Current load simulation:

```text
rate: 50
interval: 1 second
duration: 70 seconds
```

Since each scenario calls both services:

```text
50 scenarios/sec = 50 Orders requests/sec + 50 Payments requests/sec
```

## Throughput Notes

The collector is designed to protect the application APIs from telemetry backpressure:

- Telemetry ingestion uses a bounded in-memory channel.
- Full queue returns `503` quickly instead of waiting.
- Orders and Payments log telemetry delivery failures instead of failing the business request.
- The worker catches per-event failures and continues processing.
- Alert cooldown prevents repeated alert inserts during the same ongoing SLA breach.
- Development logging is reduced for high-volume framework, HTTP client, and EF command logs.

For a production-grade system, the in-memory channel should eventually be replaced with a durable queue or outbox, such as RabbitMQ, Kafka, Azure Service Bus, or a database-backed outbox.

## Common Commands

Build solution:

```powershell
dotnet build Observability-Platform.slnx
```

Run Docker stack:

```powershell
docker compose up -d --build
```

Check APIs:

```powershell
Invoke-WebRequest http://localhost:7000/health
Invoke-WebRequest http://localhost:7001/health
Invoke-WebRequest http://localhost:7130/swagger
```

Run load test:

```powershell
dotnet run --project ConsoleApp1
```

Stop Docker stack:

```powershell
docker compose down
```

## Notes

- Docker Compose runs the APIs over HTTP inside containers.
- Use `http://localhost:<port>`, not `https://localhost:<port>`, when testing Compose.
- Telemetry metrics are aggregated by endpoint and time window.
- Alerts are deduplicated by service, endpoint, and alert type for the configured cooldown period.
- The alert cooldown store is in-memory and per collector instance. If multiple collector instances are added later, alert deduplication should move to SQL Server or another shared store.
