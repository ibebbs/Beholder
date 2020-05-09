using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;

namespace Beholder.Face
{
    public interface IDetector
    {
        Task<IEnumerable<Bitmap>> ExtractFaces(Bitmap source);
    }
}
