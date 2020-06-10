using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Lensman.Running
{
    public class State : Application.IState
    {
        private readonly Navigation.IService _navigationService;

        public State(Navigation.IService navigationService)
        {
            _navigationService = navigationService;
        }

        public IObservable<Application.State.ITransition> Enter()
        {
            return Observable.Create<Application.State.ITransition>(
                async observer =>
                {
                    var activation = await _navigationService.NavigateToAsync<ViewModel>();

                    return new CompositeDisposable(
                        activation
                    );
                }
            );
        }
    }
}
