
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

namespace SecureIsolatedAzFunction
{
    public class Program
    {
        public static void Main()
        {
            var host = new HostBuilder()
                .ConfigureFunctionsWorkerDefaults(workerApplication =>
                {
                    workerApplication.UseMiddleware<AuthenticationMiddleware>();
                })
                .ConfigureAppConfiguration(configuration =>
                {
                    configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                })
                .ConfigureServices(services =>
                {
                    services.AddScoped<JwtValidation>();
                    services.AddScoped<AuthenticationProvider>();
                })
                .Build();

            host.Run();
        }
    }  
}