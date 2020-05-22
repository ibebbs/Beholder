﻿using System;
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

        Task<IEnumerable<IImage>> ExtractFaces(IImage image);

        Task<IEnumerable<IRecognition>> RecogniseFaces(IImage image);

        Task<Uri> PersistRecognised(string name, IImage image);

        Task<Uri> PersistUnrecognised(IImage image);

        Task NotifyRecognition(IPersisted recognition);
    }
}
