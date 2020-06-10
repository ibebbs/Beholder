using System;
using System.Reactive.Disposables;

namespace Lensman.Running
{
    public class ViewModel : IViewModel
    {
        public IDisposable Activate()
        {
            return Disposable.Empty;
        }
    }
}
