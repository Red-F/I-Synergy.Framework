﻿using System.Threading.Tasks;

namespace ISynergy.Framework.Mvvm.Abstractions.Services
{
    /// <summary>
    /// Interface IDownloadFileService
    /// </summary>
    public interface IDownloadFileService
    {
        /// <summary>
        /// Downloads the file asynchronous.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="filename">The filename.</param>
        /// <param name="filefilter">The filefilter.</param>
        /// <returns>Task.</returns>
        Task DownloadFileAsync(byte[] file, string filename, string filefilter);
    }
}
