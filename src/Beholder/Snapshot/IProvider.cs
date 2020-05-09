using Microsoft.Extensions.Options;
using System;
using System.Drawing;
using System.Net.Http;
using System.Threading.Tasks;

namespace Beholder.Snapshot
{
    public interface IProvider
    {
        Task<Bitmap> Get();
    }
}
