using Bebbs.Monads;
using Microsoft.ML;
using System;
using System.Drawing;

namespace Beholder.Face.Recognizer
{
    public class Implementation : IRecognizer
    {
        private readonly Invalidatable<PredictionEngine<InputModel, OutputModel>> _context;

        public Implementation()
        {
            _context = new Invalidatable<PredictionEngine<InputModel, OutputModel>>(CreateContext, DisposeContext);
        }

        private PredictionEngine<InputModel, OutputModel> CreateContext()
        {
            throw new NotImplementedException();
        }

        private void DisposeContext(PredictionEngine<InputModel, OutputModel> obj)
        {
            throw new NotImplementedException();
        }

        public IRecognition RecogniseFace(Bitmap source)
        {
            throw new NotImplementedException();
        }
    }
}
