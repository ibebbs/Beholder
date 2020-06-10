using System;

namespace Lensman
{
    public interface IViewModel
    {
        IDisposable Activate();
    }
}
