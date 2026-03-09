using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TelemetryCollector.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Alerts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServiceName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    EndPoint = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    TriggeredAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alerts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EndpointMetrics",
                columns: table => new
                {
                    ServiceName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    EndPoint = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    WindowStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalRequests = table.Column<int>(type: "int", nullable: false),
                    SuccessfulRequests = table.Column<int>(type: "int", nullable: false),
                    ClientErrorRequests = table.Column<int>(type: "int", nullable: false),
                    ServerErrorRequests = table.Column<int>(type: "int", nullable: false),
                    TotalLatencyMs = table.Column<long>(type: "bigint", precision: 18, scale: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EndpointMetrics", x => new { x.ServiceName, x.EndPoint, x.WindowStart });
                });

            migrationBuilder.CreateTable(
                name: "SlaPolicies",
                columns: table => new
                {
                    ServiceName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    EndPoint = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    MaxErrorRate = table.Column<double>(type: "float(5)", precision: 5, scale: 2, nullable: false),
                    MaxP95LatencyMs = table.Column<double>(type: "float(18)", precision: 18, scale: 3, nullable: false),
                    MaxConsecutiveHealthFailures = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SlaPolicies", x => new { x.ServiceName, x.EndPoint });
                });

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_TriggeredAt",
                table: "Alerts",
                column: "TriggeredAt");

            migrationBuilder.CreateIndex(
                name: "IX_EndpointMetrics_WindowStart",
                table: "EndpointMetrics",
                column: "WindowStart");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Alerts");

            migrationBuilder.DropTable(
                name: "EndpointMetrics");

            migrationBuilder.DropTable(
                name: "SlaPolicies");
        }
    }
}
