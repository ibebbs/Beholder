using System;

namespace Lensman.Application
{
    public interface IState
    {
        IObservable<State.ITransition> Enter();
    }
}
