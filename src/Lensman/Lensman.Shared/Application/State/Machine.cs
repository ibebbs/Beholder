using System;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Lensman.Application.State
{
    public interface IMachine
    {
        IDisposable Start();
    }

    public class Machine : IMachine
    {
        private readonly IFactory _factory;
        private readonly Subject<IState> _states;

        public Machine(IFactory factory)
        {
            _factory = factory;
            _states = new Subject<IState>();
        }

        public IDisposable Start()
        {
            // First create a stream of transitions by ...
            var transitions = _states
                // ... starting from the initializing state ...
                .StartWith(_factory.Launching())
                // ... enter the current state ...
                .Select(state => state.Enter())
                // ... subscribing to the transition observable ...
                .Switch()
                // ... and ensure only a single shared subscription is made to the transitions observable ...
                .Publish();

            // Then, for each transition type, select the new state...
            IObservable<IState> states = Observable
                .Merge(
                    transitions.OfType<Transition.ToInitializing>().Select(transition => _factory.Initializing()),
                    transitions.OfType<Transition.ToResuming>().Select(transition => _factory.Resuming()),
                    transitions.OfType<Transition.ToLogin>().Select(transition => _factory.Login()),
                    transitions.OfType<Transition.ToRecognising>().Select(transition => _factory.Recognising(transition.AsUserId)),
                    transitions.OfType<Transition.ToRunning>().Select(transition => _factory.Running()))
                //transitions.OfType<Transition.ToSuspending>().Select(transition => _factory.Suspending(transition.AggregateRoot)),
                //transitions.OfType<Transition.ToSuspended>().Select(transition => _factory.Suspended(transition.AggregateRoot)))
                // ... ensuring all transitions are serialized ...
                .ObserveOn(Scheduler.CurrentThread);


            return new CompositeDisposable(
                states.Subscribe(_states),
                transitions.Connect()
            );
        }
    }
}
