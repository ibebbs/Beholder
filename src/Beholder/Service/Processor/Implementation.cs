using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Beholder.Service.Processor
{
    public class Implementation : IProcessor
    {
        private readonly IOptions<Configuration> _options;
        private readonly ILogger<Implementation> _logger;

        public Implementation(IOptions<Configuration> options, ILogger<Implementation> logger)
        {
            _options = options;
            _logger = logger;
        }

        private async Task RunOneShot(IPipeline pipeline, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Running one shot");

            await pipeline.StartAsync(cancellationToken);
        }

        private async Task RunUntilCancelled(IPipeline pipeline, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Running until cancelled");

            while (!cancellationToken.IsCancellationRequested)
            {
                _logger.LogDebug("Pipeline.StartAsync");

                await pipeline.StartAsync(cancellationToken);
            }
        }

        public async Task RunAsync(IPipeline pipeline, CancellationToken cancellationToken)
        {
            if (_options.Value.OneShot)
            {
                await RunOneShot(pipeline, cancellationToken);
            }
            else
            {
                await RunUntilCancelled(pipeline, cancellationToken);
            }

            _logger.LogInformation("Run complete. Waiting for pipeline completion");

            await pipeline.WaitForCompletion();

            _logger.LogInformation("Pipeline completed");
        }
    }
}
