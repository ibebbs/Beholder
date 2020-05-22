using Bebbs.Monads;
using Microsoft.Extensions.ML;
using Microsoft.ML;
using System;
using System.Drawing;
using System.Linq;

namespace Beholder.Face.Recognizer
{
    public class Implementation : IRecognizer
    {
        private readonly PredictionEnginePool<InputModel, OutputModel> _predictionEnginePool;

        public Implementation(PredictionEnginePool<InputModel, OutputModel> predictionEnginePool)
        {
            _predictionEnginePool = predictionEnginePool;
        }

        public IRecognition RecogniseFace(IImage image)
        {
            var inputModel = new InputModel { Image = image.Data };

            var outputModel = _predictionEnginePool.Predict(inputModel);

            return new Recognition(image.Data, new[] { new Tag(outputModel.PredictedLabel, outputModel.Score.Max()) });
        }
    }
}
