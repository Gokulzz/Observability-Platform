using NBomber.CSharp;

public class Program
{
    public static async Task Main(string[] args)
    {
        using var httpClient = new HttpClient();
        var orderUrl = "https://localhost:7000/api/orders";
        var paymentUrl = "https://localhost:7001/api/payments";
        var scenario = Scenario.Create("OrderAndPaymentScenario", async context =>
        {
            try
            {
                var orderRequest = httpClient.PostAsync(orderUrl, null);
                var paymentRequest = httpClient.PostAsync(paymentUrl, null);
                using var orderResponse = await orderRequest;
                using var paymentResponse = await paymentRequest;

                return Response.Ok();
            }
            catch (Exception)
            {
                return Response.Fail();
            }
        })
            .WithLoadSimulations(Simulation.Inject(rate: 50, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(70)));
        NBomberRunner
            .RegisterScenarios(scenario)
            .Run();
    }
}
