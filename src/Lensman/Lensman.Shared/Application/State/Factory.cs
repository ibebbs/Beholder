using System;
using System.Collections.Generic;
using System.Text;

namespace Lensman.Application.State
{
    public interface IFactory
    {
        IState Initializing();
        IState Launching();
        IState Resuming();
        IState Login();
        IState Recognising(Guid asUserId);
        IState Running();
        IState Suspending();
        IState Suspended();
    }
    public class Factory : IFactory
    {
        private readonly Navigation.IService _navigationService;
        private readonly Data.IProvider _dataProvider;
        private readonly Event.IBus _eventBus;
        private readonly Platform.ISchedulers _platformSchedulers;

        public Factory(Navigation.IService navigationService, Data.IProvider dataProvider, Event.IBus eventBus, Platform.ISchedulers platformSchedulers)
        {
            _navigationService = navigationService;
            _dataProvider = dataProvider;
            _eventBus = eventBus;
            _platformSchedulers = platformSchedulers;
        }

        public IState Initializing()
        {
            return new Initializing(_navigationService);
        }

        public IState Launching()
        {
            return new Launching();
        }

        public IState Resuming()
        {
            return new Resuming();
        }

        public IState Login()
        {
            return new Login.State(_navigationService, _eventBus);
        }

        public IState Recognising(Guid asUserId)
        {
            return new Recognising.State(_navigationService, _dataProvider, _eventBus, _platformSchedulers, asUserId);
        }

        public IState Running()
        {
            return new Running.State(_navigationService);
        }

        public IState Suspending()
        {
            //return new Suspending();
            throw new NotImplementedException();
        }

        public IState Suspended()
        {
            //return new Suspended();
            throw new NotImplementedException();
        }
    }
}
