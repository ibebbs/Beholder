using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Lensman.Login
{
    public class State : Application.IState
    {
        private readonly Navigation.IService _navigationService;
        private readonly Lensman.Event.IBus _eventBus;

        public State(Navigation.IService navigationService, Lensman.Event.IBus eventBus)
        {
            _navigationService = navigationService;
            _eventBus = eventBus;
        }

        public IObservable<Application.State.ITransition> Enter()
        {
            return Observable.Create<Application.State.ITransition>(
                async observer =>
                {
                    var activation = await _navigationService.NavigateToAsync<ViewModel>();

                    var transitions = _eventBus
                        .GetEvent<Event.SelectedUser>()
                        .Select(@event => new Application.State.Transition.ToRecognising(@event.Id));
                    
                    return new CompositeDisposable(
                        activation,
                        transitions.Subscribe(observer)
                    );
                }
            );
        }
    }
}
