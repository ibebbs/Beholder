using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;
using PetaPoco;

namespace Director
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IDatabaseBuildConfiguration>(DatabaseConfiguration
                .Build()
                .UsingConnectionString(Configuration.GetConnectionString("PostgreSQL"))
                .UsingProviderName("Npgsql")
                .UsingCommandTimeout(180)
                .WithAutoSelect()
                .WithNamedParams()
            );

            services.AddOptions<Blob.Configuration>().ValidateDataAnnotations().Bind(Configuration.GetSection("Blob"));
            services.AddTransient<Blob.IStore, Blob.Store>();

            services.AddTransient<IDbConnection>(sp => new NpgsqlConnection());
            services.AddTransient<IDatabase>(sp => sp.GetService<IDatabaseBuildConfiguration>().Create());
            services.AddTransient<Data.IStore, Data.Store>();

            services.AddControllers();

            services.AddOpenApiDocument();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            // Register the Swagger generator and the Swagger UI middlewares
            app.UseOpenApi();
            app.UseSwaggerUi3();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
