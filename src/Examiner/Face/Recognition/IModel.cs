using Microsoft.ML;

namespace Examiner.Face.Recognition
{
    public interface IModel
    {
        public MLContext Context { get; }

        public ITransformer Model { get; }

        public DataViewSchema Schema { get; }
    }
}
