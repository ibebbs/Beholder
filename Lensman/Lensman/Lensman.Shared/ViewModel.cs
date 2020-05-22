using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Lensman.Shared
{
    public class ViewModel : INotifyPropertyChanged
    {
        private readonly MVx.Observable.Command _ian = new MVx.Observable.Command();
        private readonly MVx.Observable.Command _rachel = new MVx.Observable.Command();
        private readonly MVx.Observable.Command _mia = new MVx.Observable.Command();
        private readonly MVx.Observable.Command _max = new MVx.Observable.Command();
        private readonly MVx.Observable.Command _alan = new MVx.Observable.Command();
        private readonly MVx.Observable.Command _notAFace = new MVx.Observable.Command();
        private readonly MVx.Observable.Command _undo = new MVx.Observable.Command();

        private readonly MVx.Observable.Property<Face> _current;

        private readonly BehaviorSubject<Identification> _submitted = new BehaviorSubject<Identification>(Identification.Empty);
        private readonly FaceProvider _provider;
        private readonly IScheduler _scheduler;

        private IDisposable _behaviours;

        public event PropertyChangedEventHandler PropertyChanged;

        public ViewModel(IScheduler scheduler)
        {
            _provider = new FaceProvider();
            _scheduler = scheduler;

            _current = new MVx.Observable.Property<Face>(nameof(Face), args => PropertyChanged?.Invoke(this, args));
        }

        private async Task<(bool, Face)> GetFaceAsync(IAsyncEnumerator<Face> enumerator)
        {
            var available = await enumerator.MoveNextAsync();

            return (available, enumerator.Current);
        }

        private IDisposable ShouldGetNewImageOnSubscriptionAndWhenItemCompleted()
        {
            return ObservableExtensions
                .UsingAsync(
                    () => _provider.Faces().GetAsyncEnumerator(),
                    enumerator => _submitted
                        .SelectMany(_ => GetFaceAsync(enumerator))
                        .TakeWhile(tuple => tuple.Item1)
                        .Select(tuple => tuple.Item2))
                .ObserveOn(_scheduler)
                .Subscribe(_current);
        }

        private IDisposable ShouldMoveImageToAppropriateFolderWhenButtonClicked()
        {
            return Observable
                .Merge(
                    _ian.Select(_ => "ian"),
                    _rachel.Select(_ => "rachel"),
                    _mia.Select(_ => "mia"),
                    _max.Select(_ => "max"),
                    _alan.Select(_ => "alan"),
                    _notAFace.Select(_ => "none"))
                .WithLatestFrom(_current, (person, item) => new Identification { Person = person, BlobName = item.Name })
                .Do(identification => _provider.MoveItemToPerson(identification))
                .Subscribe(_submitted);
        }

        private IDisposable ShouldEnableButtonsWhenCurrentItemAvailable()
        {
            var observable = _current
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

        private IDisposable ShouldDisableButtonsWhenButtonClicked()
        {
            var observable = Observable
                   .Merge(
                       _ian.Select(_ => false),
                       _rachel.Select(_ => false),
                       _mia.Select(_ => false),
                       _max.Select(_ => false),
                       _alan.Select(_ => false),
                       _notAFace.Select(_ => false))
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

        private IDisposable ShouldEnableUndoWhenIdentificationSubmitted()
        {
            return _submitted
                .Select(identification => !object.Equals(identification, Identification.Empty))
                .Subscribe(_undo);
        }

        public void Activate()
        {
            _behaviours = new CompositeDisposable(
                ShouldGetNewImageOnSubscriptionAndWhenItemCompleted(),
                ShouldEnableButtonsWhenCurrentItemAvailable(),
                ShouldDisableButtonsWhenButtonClicked(),
                ShouldMoveImageToAppropriateFolderWhenButtonClicked(),
                ShouldEnableUndoWhenIdentificationSubmitted()
            );
        }

        public ICommand Ian => _ian;

        public ICommand Rachel => _rachel;

        public ICommand Mia => _mia;

        public ICommand Max => _max;

        public ICommand Alan => _alan;

        public ICommand NotAFace => _notAFace;

        public ICommand Undo => _undo;

        public Face Face
        {
            get { return _current.Get(); }
        }
    }
}
