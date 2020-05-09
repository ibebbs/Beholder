using System;
using System.Drawing;

namespace Beholder
{
    public interface IImage : IDisposable
    {
        Bitmap Bitmap { get; }
    }
}
