using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Beholder.Service
{
    public class Implementation : IHostedService, IDisposable
    {
        private readonly Pipeline.IFactory _pipelineFactory;
        private readonly IProcessor _processor;
        private readonly CancellationTokenSource _cancellationTokenSource;

        private Task _processorTask;

        public Implementation(Pipeline.IFactory pipelineFactory, IProcessor processor, ILogger<Implementation> logger)
        {
            _pipelineFactory = pipelineFactory;
            _processor = processor;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            IPipeline pipeline = await _pipelineFactory.CreateAsync();

            // Store the task we're executing
            _processorTask = _processor.RunAsync(pipeline, _cancellationTokenSource.Token);

            // If the task is completed then return it, this will bubble cancellation and failure to the caller
            if (_processorTask.IsCompleted)
            {
                await _processorTask;
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_processorTask != null)
            {
                _cancellationTokenSource.Cancel();

                await Task.WhenAny(_processorTask, Task.Delay(Timeout.Infinite, cancellationToken));
            }
        }
    }
}
