using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Beholder.Service.Pipeline.Factory
{
    public class Implementation : IFactory
    {
        private readonly Functions.IFactory _functionsFactory;
        private readonly ILogger<Pipeline.Implementation> _logger;

        public Implementation(Functions.IFactory functionsFactory, ILogger<Pipeline.Implementation> logger)
        {
            _functionsFactory = functionsFactory;
            _logger = logger;
        }

        public async Task<IPipeline> CreateAsync()
        {
            var functions = await _functionsFactory.Create(_logger);

            return new Pipeline.Implementation(functions, _logger);
        }
    }
}
