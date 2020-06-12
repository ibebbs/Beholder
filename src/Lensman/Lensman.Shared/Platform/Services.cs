using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Net.Http;
using System.Reflection;

namespace Lensman.Platform
{
    public partial class Services
    {
        public static readonly Services Service = new Services();

        private readonly ServiceCollection _serviceCollection;
        private readonly Lazy<IServiceProvider> _serviceProvider;

        private Services()
        {
            _serviceCollection = new ServiceCollection();
            _serviceProvider = new Lazy<IServiceProvider>(() => _serviceCollection.BuildServiceProvider());
        }

        private void RegisterGlobalServices(IServiceCollection services)
        {
            services.AddSingleton<IViewLocator, ViewLocator>();

            services.AddSingleton<System.Net.Http.IHttpClientFactory, CustomHttpClientFactory>();

            services.AddHttpClient<Director.Client.IFacesClient, Director.Client.FacesClient>(
                serviceProvider => new HttpClient(CustomHttpClientFactory.Handler) { BaseAddress = new Uri("http://localhost:5000") }
            );

            services.AddSingleton<Data.IProvider, Data.Provider>();
            services.AddSingleton<Event.IBus, Event.Bus>();
            services.AddSingleton<Navigation.IService, Navigation.Service>();
            services.AddSingleton<Platform.ISchedulers, Platform.Schedulers>();
            services.AddSingleton<Application.State.IFactory, Application.State.Factory>();
            services.AddSingleton<Application.State.IMachine, Application.State.Machine>();

            var viewModels = Assembly.GetExecutingAssembly().GetTypes()
                .Where(type => typeof(IViewModel).IsAssignableFrom(type) && !type.IsInterface);

            foreach (var viewModel in viewModels)
            {
                services.AddTransient(viewModel);
            }
        }

        partial void RegisterPlatformServices(IServiceCollection services);

        public void PerformRegistration()
        {
            if (_serviceProvider.IsValueCreated) throw new InvalidOperationException("You cannot register services after the service provider has been created");

            RegisterGlobalServices(_serviceCollection);
            RegisterPlatformServices(_serviceCollection);
        }

        public IServiceProvider Provider => _serviceProvider.Value;
    }
}
