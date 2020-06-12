using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Lensman.Recognising
{
    public class ViewModel : IViewModel, INotifyPropertyChanged
    {
        private readonly Data.IProvider _dataProvider;
        private readonly Lensman.Event.IBus _eventBus;
        private readonly Guid _userId;
        private readonly Platform.ISchedulers _schedulers;
        private readonly BehaviorSubject<Director.Client.Recognition> _recognised;
        private readonly MVx.Observable.Property<Director.Client.Face> _face;

        private readonly ILogger<ViewModel> _logger;

        private readonly MVx.Observable.Command _ian;
        private readonly MVx.Observable.Command _rachel;
        private readonly MVx.Observable.Command _mia;
        private readonly MVx.Observable.Command _max;
        private readonly MVx.Observable.Command _alan;
        private readonly MVx.Observable.Command _notAFace;

        public event PropertyChangedEventHandler PropertyChanged;

        public ViewModel(Data.IProvider dataProvider, Lensman.Event.IBus eventBus, Platform.ISchedulers schedulers, Guid userId)
        {
            _dataProvider = dataProvider;
            _eventBus = eventBus;
            _userId = userId;
            _schedulers = schedulers;

            _logger = global::Uno.Extensions.LogExtensionPoint.AmbientLoggerFactory.CreateLogger<ViewModel>();

            _recognised = new BehaviorSubject<Director.Client.Recognition>(null);
            _face = new MVx.Observable.Property<Director.Client.Face>(nameof(Face), args => PropertyChanged?.Invoke(this, args));

            _ian = new MVx.Observable.Command();
            _rachel = new MVx.Observable.Command();
            _mia = new MVx.Observable.Command();
            _max = new MVx.Observable.Command();
            _alan = new MVx.Observable.Command();
            _notAFace = new MVx.Observable.Command();
        }

        private async Task<(bool, Director.Client.Face)> GetFaceAsync(IAsyncEnumerator<Director.Client.Face> enumerator)
        {
            var available = await enumerator.MoveNextAsync();

            return (available, enumerator.Current);
        }

        private IDisposable ShouldRetrieveFaceWhenActivatedAndAfterRecognition()
        {
            //return ObservableExtensions
            //    .UsingAsync(
            //        () => _dataProvider.UnrecognisedBy(_userId, page => 1).GetAsyncEnumerator(),
            //        enumerator => _recognised
            //            .SelectMany(_ => GetFaceAsync(enumerator))
            //            .TakeWhile(tuple => tuple.Item1)
            //            .Select(tuple => tuple.Item2))
            //    .ObserveOn(_schedulers.Dispatcher)
            //    .Subscribe(_face);

            return _recognised
                .SelectMany(_ => _dataProvider.UnrecognisedBy(_userId))
                .Do(face => _logger.LogInformation($"Unrecognised: {face.Id} @ {face.Uri}"))
                .ObserveOn(_schedulers.Dispatcher)
                .Subscribe(_face);
        }

        private IDisposable ShouldEnableButtonsWhenFaceAvailable()
        {
            var observable = _face
                .Select(_ => true)
                .Publish();

            return new CompositeDisposable(
                observable.Subscribe(_ian),
                observable.Subscribe(_rachel),
                observable.Subscribe(_mia),
                observable.Subscribe(_max),
                observable.Subscribe(_alan),
                observable.Subscribe(_notAFace),
                observable.Connect()
            );
        }

        private IDisposable ShouldRecogniseFaceWhenButtonClicked()
        {
            return Observable
                .Merge(
                    _ian.Select(_ => "ian"),
                    _rachel.Select(_ => "rachel"),
                    _mia.Select(_ => "mia"),
                    _max.Select(_ => "max"),
                    _alan.Select(_ => "alan"),
                    _notAFace.Select(_ => "none"))
                .WithLatestFrom(_face, (person, face) => (Person: person, Face: face))
                .SelectMany(tuple => _dataProvider.Recognise(tuple.Face.Id, _userId, tuple.Person, 1f))
                .Subscribe(_recognised);
        }

        public IDisposable Activate()
        {
            return new CompositeDisposable(
                ShouldRetrieveFaceWhenActivatedAndAfterRecognition(),
                ShouldEnableButtonsWhenFaceAvailable(),
                ShouldRecogniseFaceWhenButtonClicked()
            );
        }

        public Director.Client.Face Face => _face.Get();

        public ICommand Ian => _ian;

        public ICommand Rachel => _rachel;

        public ICommand Mia => _mia;

        public ICommand Max => _max;

        public ICommand Alan => _alan;

        public ICommand NotAFace => _notAFace;
    }
}
