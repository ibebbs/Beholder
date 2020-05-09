using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.ML.Data;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
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
        private readonly Image.IFactory _imageFactory;
        private readonly ILogger<Implementation> _logger;
        private readonly ITargetBlock<string> _entryPoint;
        private readonly Task _completion;

        public Implementation(IFunctions functions, Image.IFactory imageFactory, ILogger<Implementation> logger)
        {
            _functions = functions;
            _imageFactory = imageFactory;
            _logger = logger;

            (_entryPoint, _completion) = CreatePipeline();
        }

        private async Task<bool> PersistFace(IImage image)
        {
            try
            {
                await _functions.PersistFace(image);

                return true;
            }
            catch (Exception)
            {
                return true;
            }
        }

        private (IImage, IImage) CloneImage(IImage source)
        {
            var clone = _imageFactory.Clone(source);

            return (source, clone);
        }

        private (ITargetBlock<string>, Task) CreatePipeline()
        {
            var linkOptions = new DataflowLinkOptions { PropagateCompletion = true };

            TransformManyBlock<object, IImage> fetchBlock = new TransformManyBlock<object, IImage>(_ => _functions.Fetch(), new ExecutionDataflowBlockOptions { BoundedCapacity = 2, MaxDegreeOfParallelism = 1 });
            TransformManyBlock<IImage, IImage> detectFacesBlock = new TransformManyBlock<IImage, IImage>(source => _functions.ExtractFaces(source));

            TransformBlock<IImage, (IImage, IImage)> splitBlock = new TransformBlock<IImage, (IImage, IImage)>(CloneImage);
            BroadcastBlock<(IImage, IImage)> broadcastBlock1 = new BroadcastBlock<(IImage, IImage)>(tuple => tuple);

            TransformBlock<(IImage, IImage), IImage> useLeftBlock = new TransformBlock<(IImage, IImage), IImage>(tuple => tuple.Item1);
            ActionBlock<IImage> persistFaceBlock = new ActionBlock<IImage>(source => _functions.PersistFace(source), new ExecutionDataflowBlockOptions { BoundedCapacity = 10, MaxDegreeOfParallelism = 1 });

            TransformBlock<(IImage, IImage), IImage> useRightBlock = new TransformBlock<(IImage, IImage), IImage>(tuple => tuple.Item2);
            TransformManyBlock<IImage, IRecognition> recogniseFacesBlock = new TransformManyBlock<IImage, IRecognition>(source => _functions.RecogniseFaces(source));

            BroadcastBlock<IRecognition> broadcastBlock2 = new BroadcastBlock<IRecognition>(recognition => recognition);

            ActionBlock<IRecognition> persistRecognitionBlock = new ActionBlock<IRecognition>(recognition => _functions.PersistFacialRecognition(recognition), new ExecutionDataflowBlockOptions { BoundedCapacity = 10, MaxDegreeOfParallelism = 1 });
            ActionBlock<IRecognition> notifyRecognitionBlock = new ActionBlock<IRecognition>(recognition => _functions.NotifyFacialRecognition(recognition), new ExecutionDataflowBlockOptions { BoundedCapacity = 10, MaxDegreeOfParallelism = 1 });

            fetchBlock.LinkTo(detectFacesBlock, linkOptions);
            detectFacesBlock.LinkTo(splitBlock, linkOptions);
            splitBlock.LinkTo(broadcastBlock1, linkOptions);
            broadcastBlock1.LinkTo(useLeftBlock, linkOptions);
            broadcastBlock1.LinkTo(useRightBlock, linkOptions);
            useLeftBlock.LinkTo(persistFaceBlock, linkOptions);
            useRightBlock.LinkTo(recogniseFacesBlock, linkOptions);
            recogniseFacesBlock.LinkTo(broadcastBlock2, linkOptions);
            broadcastBlock2.LinkTo(persistRecognitionBlock, linkOptions);
            broadcastBlock2.LinkTo(notifyRecognitionBlock, linkOptions);

            return (fetchBlock, Task.WhenAll(persistFaceBlock.Completion, persistRecognitionBlock.Completion, notifyRecognitionBlock.Completion));
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
