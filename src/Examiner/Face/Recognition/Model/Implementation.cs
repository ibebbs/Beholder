using Microsoft.ML;

namespace Examiner.Face.Recognition.Model
{
    public class Implementation : IModel
    {
        public MLContext Context { get; set; }

        public ITransformer Model { get; set; }

        public DataViewSchema Schema { get; set; }
    }
}
