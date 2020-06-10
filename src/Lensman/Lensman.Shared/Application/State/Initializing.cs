using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;

namespace Lensman.Application.State
{
    public class Initializing : IState
    {
        private readonly Navigation.IService _navigationService;

        public Initializing(Navigation.IService navigationService)
        {
            _navigationService = navigationService;
        }

        public IObservable<ITransition> Enter()
        {
            return Observable.Create<ITransition>(
                async observer =>
                {
                    await _navigationService.InitializeAsync();

                    return Observable
                        .Return(new Transition.ToResuming())
                        .Subscribe(observer);
                }
            );
        }
    }
}
