using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Lensman.Recognising
{
    public class State : Application.IState
    {
        private readonly Navigation.IService _navigationService;
        private readonly Lensman.Event.IBus _eventBus;
        private readonly Data.IProvider _dataProvider;
        private readonly Platform.ISchedulers _schedulers;
        private readonly Guid _userId;

        public State(Navigation.IService navigationService, Data.IProvider dataProvider, Lensman.Event.IBus eventBus, Platform.ISchedulers schedulers, Guid userId)
        {
            _navigationService = navigationService;
            _eventBus = eventBus;
            _dataProvider = dataProvider;
            _schedulers = schedulers;
            _userId = userId;
        }

        public IObservable<Application.State.ITransition> Enter()
        {
            return Observable.Create<Application.State.ITransition>(
                async observer =>
                {
                    var activation = await _navigationService.NavigateToAsync(new ViewModel(_dataProvider, _eventBus, _schedulers, _userId));
                    
                    return new CompositeDisposable(
                        activation
                    );
                }
            );
        }
    }
}
