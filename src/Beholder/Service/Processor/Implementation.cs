using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Beholder.Service.Processor
{
    public class Implementation : IProcessor
    {
        private readonly ILogger<Implementation> _logger;

        public Implementation(ILogger<Implementation> logger)
        {
            _logger = logger;
        }

        public async Task RunAsync(IPipeline pipeline, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Entering message pump");

            while (!cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation("Pump");

                await pipeline.StartAsync(cancellationToken);
            }

            _logger.LogInformation("Exited message pump. Waiting for pipeline completion");

            await pipeline.WaitForCompletion();

            _logger.LogInformation("Pipeline completed");
        }
    }
}
