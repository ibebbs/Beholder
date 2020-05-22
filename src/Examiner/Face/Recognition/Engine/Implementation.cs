using Examiner.Face.Data;
using Microsoft.Extensions.Logging;
using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Examiner.Face.Recognition.Engine
{
    public class Implementation : IEngine
    {
        private readonly ILogger<Implementation> _logger;

        public Implementation(ILogger<Implementation> logger)
        {
            _logger = logger;
        }

        private void LogContext(object sender, LoggingEventArgs e)
        {
            switch (e.Kind)
            {
                case Microsoft.ML.Runtime.ChannelMessageKind.Error:
                    _logger.LogError(e.Message);
                    break;
                case Microsoft.ML.Runtime.ChannelMessageKind.Warning:
                    _logger.LogWarning(e.Message);
                    break;
                case Microsoft.ML.Runtime.ChannelMessageKind.Info:
                    _logger.LogInformation(e.Message);
                    break;
                case Microsoft.ML.Runtime.ChannelMessageKind.Trace:
                    _logger.LogTrace(e.Message);
                    break;
            }
        }

        public Task<IModel> TrainAsync(IAsyncEnumerable<Item> source, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew<IModel>(
                () =>
                {
                    var context = new MLContext();
                    context.Log += LogContext;

                    var enumerable = source
                        .Select(item => new TrainingData { Label = item.Name, Image = item.Image })
                        .ToEnumerable();

                    var trainingData = context.Data.LoadFromEnumerable(enumerable);

                    var pipeline = context.Transforms.Conversion.MapValueToKey("Label", "Label")
                        .Append(context.MulticlassClassification.Trainers.ImageClassification(new Microsoft.ML.Vision.ImageClassificationTrainer.Options() { LabelColumnName = "Label", FeatureColumnName = "Image" }))
                        .Append(context.Transforms.Conversion.MapKeyToValue("PredictedLabel", "PredictedLabel"));

                    var trainedModel = pipeline.Fit(trainingData);

                    context.Log -= LogContext;
                    return new Model.Implementation { Context = context, Model = trainedModel, Schema = trainingData.Schema };
                },
                cancellationToken,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default
            );
        }
    }
}
