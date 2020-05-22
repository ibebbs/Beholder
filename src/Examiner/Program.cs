using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Examiner
{
    class Program
    {
        public static IServiceCollection ConfigureServices(HostBuilderContext hostContext, IServiceCollection services)
        {
            services.AddSingleton<Face.Data.IProvider, Face.Data.Provider>();
            services.AddSingleton<Face.Recognition.IEngine, Face.Recognition.Engine.Implementation>();
            services.AddSingleton<Face.Recognition.Model.IPersistor, Face.Recognition.Model.Persistor>();
            services.AddOptions<Face.Configuration>().ValidateDataAnnotations().Bind(hostContext.Configuration.GetSection("Face"));
            services.AddSingleton<Persistence.IProvider, Persistence.Provider>();
            services.AddSingleton<Service.State.IProvider, Service.State.Provider>();
            services.AddSingleton<Service.IProcessor, Service.Processor.Implementation>();
            services.AddOptions<Service.Configuration>().ValidateDataAnnotations().Bind(hostContext.Configuration.GetSection("Service"));
            services.AddHostedService<Service.Implementation>();

            return services;
        }

        static async Task Main(string[] args)
        {
            var builder = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(
                    (hostingContext, config) =>
                    {
                        config.AddEnvironmentVariables("Examiner:");
                        if (args != null)
                        {
                            config.AddCommandLine(args);
                        }
                    })
                .ConfigureServices((hostContext, services) => ConfigureServices(hostContext, services))
                .ConfigureLogging(
                (hostingContext, logging) =>
                {
                    logging.AddConsole();
                });

            try
            {
                await builder
                    .UseConsoleLifetime()
                    .Build()
                    .ValidateConfiguration<Service.Configuration, Face.Configuration>()
                    .RunAsync();
            }
            catch (ConfigurationValidationException e)
            {
                Console.WriteLine($"One or more configuration errors occured:{Environment.NewLine}{e.Message}");

                if (System.Diagnostics.Debugger.IsAttached)
                {
                    Console.WriteLine("Hit <Enter> to exit.");
                    Console.ReadLine();
                }
            }
        }
    }
}
