using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Examiner.Service
{
    public class Implementation : BackgroundService
    {
        private readonly State.IProvider _stateProvider;
        private readonly IProcessor _processor;
        private readonly IHostApplicationLifetime _lifetime;

        public Implementation(State.IProvider stateProvider, IProcessor processor, IHostApplicationLifetime lifetime)
        {
            _stateProvider = stateProvider;
            _processor = processor;
            _lifetime = lifetime;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var snapshot = await _stateProvider.GetStateSnapshotAsync(stoppingToken);

            if (await _processor.RequiresProcessingAsync(snapshot, stoppingToken))
            {
                var newSnapshot = await _processor.ProcessAsync(snapshot, stoppingToken);
                await _stateProvider.SetStateSnapshotAsync(newSnapshot, stoppingToken);
            }

            _lifetime.StopApplication();
        }
    }
}
