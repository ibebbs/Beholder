using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;

namespace Lensman.Application.State
{
    public class Resuming : IState
    {
        public IObservable<ITransition> Enter()
        {
            return Observable.Return(new Transition.ToLogin());
        }
    }
}
