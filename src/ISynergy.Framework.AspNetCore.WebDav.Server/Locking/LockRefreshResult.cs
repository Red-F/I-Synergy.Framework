﻿using System.Collections.Generic;
using ISynergy.Framework.AspNetCore.WebDav.Server.Model;

namespace ISynergy.Framework.AspNetCore.WebDav.Server.Locking
{
    /// <summary>
    /// The result of a LOCK refresh operation
    /// </summary>
    public class LockRefreshResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LockRefreshResult"/> class.
        /// </summary>
        /// <param name="errorResponse">The error to return</param>
        public LockRefreshResult(response errorResponse)
        {
            ErrorResponse = errorResponse;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LockRefreshResult"/> class.
        /// </summary>
        /// <param name="refreshedLocks">The active locks that could be refreshed</param>
        public LockRefreshResult(IReadOnlyCollection<IActiveLock> refreshedLocks)
        {
            RefreshedLocks = refreshedLocks;
        }

        /// <summary>
        /// Gets the active lock when locking succeeded
        /// </summary>
        public IReadOnlyCollection<IActiveLock> RefreshedLocks { get; }

        /// <summary>
        /// Gets the error response to return
        /// </summary>
        public response ErrorResponse { get; }
    }
}
