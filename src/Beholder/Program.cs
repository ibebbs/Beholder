﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Beholder
{
    public class Program
    {
        public static IServiceCollection ConfigureServices(HostBuilderContext hostContext, IServiceCollection services)
        {
            services.AddSingleton<Image.IFactory, Image.Factory.Implementation>();
            services.AddOptions<Snapshot.Configuration>().ValidateDataAnnotations().Bind(hostContext.Configuration.GetSection("Snapshot"));
            services.AddSingleton<Face.IDetector, Face.Detector.Implementation>();
            services.AddOptions<Persistence.Configuration>().ValidateDataAnnotations().Bind(hostContext.Configuration.GetSection("Persistence"));
            services.AddSingleton<Persistence.IProvider, Persistence.Provider.Implementation>();
            services.AddHttpClient<Snapshot.IProvider, Snapshot.Provider.Implementation>();
            services.AddOptions<Service.Configuration>().ValidateDataAnnotations().Bind(hostContext.Configuration.GetSection("Service"));
            services.AddSingleton<Service.Pipeline.Functions.IFactory, Service.Pipeline.Functions.Factory.Implementation>();
            services.AddSingleton<Service.Pipeline.IFactory, Service.Pipeline.Factory.Implementation>();
            services.AddSingleton<Service.IProcessor, Service.Processor.Implementation>();
            services.AddHostedService<Service.Implementation>();

            return services;
        }

        static async Task Main(string[] args)
        {
            var builder = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(
                    (hostingContext, config) =>
                    {
                        config.AddEnvironmentVariables("Beholder:");
                        if (args != null)
                        {
                            config.AddCommandLine(args);
                        }
                    })
                .ConfigureServices((hostContext, services) => ConfigureServices(hostContext,services))
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
                    .ValidateConfiguration<Service.Configuration, Snapshot.Configuration, Persistence.Configuration>()
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
