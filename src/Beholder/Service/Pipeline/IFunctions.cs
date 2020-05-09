using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Beholder.Service.Pipeline
{
    /// <summary>
    /// A simple facade for interacting with other components
    /// </summary>
    public interface IFunctions
    {
        Task<IEnumerable<IImage>> Fetch();

        Task<IEnumerable<IImage>> ExtractFaces(IImage bitmap);

        Task PersistFace(IImage bitmap);

        Task<IEnumerable<IRecognition>> RecogniseFaces(IImage bitmap);

        Task PersistFacialRecognition(IRecognition recognition);

        Task NotifyFacialRecognition(IRecognition recognition);
    }
}
