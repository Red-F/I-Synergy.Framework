﻿using System.Threading;
using System.Threading.Tasks;
using ISynergy.Framework.AspNetCore.WebDav.Server.Model;
using ISynergy.Framework.AspNetCore.WebDav.Server.Model.Headers;

namespace ISynergy.Framework.AspNetCore.WebDav.Server.Handlers
{
    /// <summary>
    /// Interface for the <c>LOCK</c> handler
    /// </summary>
    public interface ILockHandler : IClass2Handler
    {
        /// <summary>
        /// Creates a lock for the given <paramref name="path"/> using the information in <paramref name="info"/>.
        /// </summary>
        /// <param name="path">The path to create the lock for</param>
        /// <param name="info">The additional information used to create the lock</param>
        /// <param name="cancellationToken">The cancellcation token</param>
        /// <returns>The result of the operation</returns>
        Task<IWebDavResult> LockAsync(string path, lockinfo info, CancellationToken cancellationToken);

        /// <summary>
        /// Refreshes a lock for the given <paramref name="path"/> using the lock identified by the <paramref name="ifHeader"/>.
        /// </summary>
        /// <param name="path">The path to create the lock for</param>
        /// <param name="ifHeader">The <c>If</c> header used to identify the lock</param>
        /// <param name="timeoutHeader">The new timeout values</param>
        /// <param name="cancellationToken">The cancellcation token</param>
        /// <returns>The result of the operation</returns>
        Task<IWebDavResult> RefreshLockAsync(string path, IfHeader ifHeader, TimeoutHeader timeoutHeader, CancellationToken cancellationToken);
    }
}
