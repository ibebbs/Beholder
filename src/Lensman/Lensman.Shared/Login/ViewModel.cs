using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;

namespace Lensman.Login
{
    public class ViewModel : IViewModel
    {
        private readonly Lensman.Event.IBus _eventBus;

        private readonly MVx.Observable.Command _ian;
        private readonly MVx.Observable.Command _rachel;
        private readonly MVx.Observable.Command _mia;
        private readonly MVx.Observable.Command _max;

        public ViewModel(Lensman.Event.IBus eventBus)
        {
            _eventBus = eventBus;

            _ian = new MVx.Observable.Command(true);
            _rachel = new MVx.Observable.Command(true);
            _mia = new MVx.Observable.Command(true);
            _max = new MVx.Observable.Command(true);
        }

        private IDisposable ShouldLoginUsingSelectedUserWhenUserClicked()
        {
            return Observable
                .Merge(
                    _ian.Select(_ => User.Constants.Ian),
                    _rachel.Select(_ => User.Constants.Rachel),
                    _mia.Select(_ => User.Constants.Mia),
                    _max.Select(_ => User.Constants.Max))
                .Select(id => new Event.SelectedUser(id))
                .Subscribe(_eventBus.Publish);
        }

        public IDisposable Activate()
        {
            return new CompositeDisposable(
                ShouldLoginUsingSelectedUserWhenUserClicked()
            );
        }

        public ICommand Ian => _ian;
        public ICommand Rachel => _rachel;
        public ICommand Mia => _mia;
        public ICommand Max => _max;
    }
}
