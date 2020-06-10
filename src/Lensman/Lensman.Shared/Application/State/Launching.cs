using System;
using System.Reactive.Linq;

namespace Lensman.Application.State
{
    public class Launching : IState
    {
        public IObservable<ITransition> Enter()
        {
            return Observable.Return(new Transition.ToInitializing());
        }
    }
}
