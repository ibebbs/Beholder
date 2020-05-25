using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.ML.Data;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Beholder.Service.Pipeline
{

    public static class PipelineActions
    {
    }

    public class Implementation : IPipeline
    {
        private readonly IFunctions _functions;
        private readonly IOptions<Configuration> _options;
        private readonly ILogger<Implementation> _logger;
        private readonly ITargetBlock<string> _entryPoint;
        private readonly Task _completion;

        public Implementation(IFunctions functions, IOptions<Configuration> options, ILogger<Implementation> logger)
        {
            _functions = functions;
            _options = options;
            _logger = logger;

            (_entryPoint, _completion) = CreatePipeline();
        }

        private (ITargetBlock<string>, Task) CreatePipeline()
        {
            var linkOptions = new DataflowLinkOptions { PropagateCompletion = true };

            TransformManyBlock<object, IImage> fetchBlock = new TransformManyBlock<object, IImage>(_ => _functions.Fetch(), new ExecutionDataflowBlockOptions { BoundedCapacity = 2, MaxDegreeOfParallelism = 1 });
            TransformManyBlock<IImage, IImage> detectFacesBlock = new TransformManyBlock<IImage, IImage>(source => _functions.ExtractFaces(source));
            TransformManyBlock<IImage, IRecognition> recogniseFacesBlock = new TransformManyBlock<IImage, IRecognition>(source => _functions.RecogniseFaces(source));
            TransformBlock<IRecognition, IPersistedRecognition> persistRecognitionBlock = new TransformBlock<IRecognition, IPersistedRecognition>(recognition => _functions.PersistRecognition(recognition), new ExecutionDataflowBlockOptions { BoundedCapacity = 10, MaxDegreeOfParallelism = 1 });
            ActionBlock<IPersistedRecognition> notifyRecognitionBlock = new ActionBlock<IPersistedRecognition>(recognition => _functions.NotifyRecognition(recognition), new ExecutionDataflowBlockOptions { BoundedCapacity = 10, MaxDegreeOfParallelism = 1 });

            fetchBlock.LinkTo(detectFacesBlock, linkOptions);
            detectFacesBlock.LinkTo(recogniseFacesBlock, linkOptions);
            recogniseFacesBlock.LinkTo(persistRecognitionBlock, linkOptions);
            persistRecognitionBlock.LinkTo(notifyRecognitionBlock, linkOptions);

            return (fetchBlock, notifyRecognitionBlock.Completion);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return _entryPoint.SendAsync(null, cancellationToken);
        }

        public Task WaitForCompletion()
        {
            _entryPoint.Complete();

            return _completion;
        }
    }
}
