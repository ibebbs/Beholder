using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ML;
using System.Linq;

namespace Beholder.Face.Recognizer
{
    public class Implementation : IRecognizer
    {
        private readonly PredictionEnginePool<InputModel, OutputModel> _predictionEnginePool;
        private readonly ILogger<Implementation> _logger;

        public Implementation(PredictionEnginePool<InputModel, OutputModel> predictionEnginePool, ILogger<Implementation> logger)
        {
            _predictionEnginePool = predictionEnginePool;
            _logger = logger;
        }

        public IRecognition RecogniseFace(IImage image)
        {
            _logger.LogInformation("Recognising Face");

            var inputModel = new InputModel { Image = image.Data };

            var outputModel = _predictionEnginePool.Predict(inputModel);

            var label = outputModel.PredictedLabel;
            var score = outputModel.Score.Max();

            _logger.LogInformation("Recognition complete. Identified as {0} with confidence {1:P2}", label, score);

            return new Recognition(image.Data, new[] { new Tag(label, score) });
        }
    }
}
