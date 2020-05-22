using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Examiner.Face.Recognition.Model
{
    public interface IPersistor
    {
        Task SaveModelAsync(IModel model, CancellationToken ignored);
    }

    public class Persistor : IPersistor
    {
        private readonly Persistence.IProvider _persistenceProvider;
        private readonly IOptions<Configuration> _options;

        public Persistor(Persistence.IProvider persistenceProvider, IOptions<Configuration> options)
        {
            _persistenceProvider = persistenceProvider;
            _options = options;

        }
        public async Task SaveModelAsync(IModel model, CancellationToken cancellationToken)
        {
            Func<Stream, Task> saveModel = stream =>
            {
                model.Context.Model.Save(model.Model, model.Schema, stream);
                return Task.CompletedTask;
            };

            await _persistenceProvider.SetBlobContentAsync(_options.Value.ConnectionString, _options.Value.FaceRecognitionModelContainerName, "Model.zip", saveModel, cancellationToken);
        }
    }
}
