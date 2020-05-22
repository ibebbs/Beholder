using System;
using System.Threading;
using System.Threading.Tasks;

namespace Examiner.Service.Processor
{
    public class Implementation : IProcessor
    {
        private readonly Face.Data.IProvider _dataProvider;
        private readonly Face.Recognition.IEngine _recognitionEngine;
        private readonly Face.Recognition.Model.IPersistor _modelPersistor;

        public Implementation(Face.Data.IProvider dataProvider, Face.Recognition.IEngine recognitionEngine, Face.Recognition.Model.IPersistor modelPersistor)
        {
            _dataProvider = dataProvider;
            _recognitionEngine = recognitionEngine;
            _modelPersistor = modelPersistor;
        }

        public async Task<bool> RequiresProcessingAsync(State.Snapshot snapshot, CancellationToken cancellationToken)
        {
            var lastChangeToTrainingData = await _dataProvider.GetDateOfLastChangeAsync(cancellationToken);

            return ((lastChangeToTrainingData ?? DateTimeOffset.MinValue) > (snapshot.LastTrainingDataUpdate ?? DateTimeOffset.MinValue));
        }

        public async Task<State.Snapshot> ProcessAsync(State.Snapshot snapshot, CancellationToken cancellationToken)
        {
            var lastChangeToTrainingData = await _dataProvider.GetDateOfLastChangeAsync(cancellationToken);

            var faceData = _dataProvider.GetBalancedItems(cancellationToken);

            var model = await _recognitionEngine.TrainAsync(faceData, cancellationToken);

            await _modelPersistor.SaveModelAsync(model, cancellationToken);

            return new State.Snapshot { LastTrainingDataUpdate = lastChangeToTrainingData };
        }
    }
}
