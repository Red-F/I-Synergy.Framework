﻿using System;

namespace ISynergy.Framework.AspNetCore.Authentication.Exceptions
{
    /// <summary>
    /// Class DuplicateClaimException.
    /// Implements the <see cref="ClaimAuthorizationException" />
    /// </summary>
    /// <seealso cref="ClaimAuthorizationException" />
    public class DuplicateClaimException : ClaimAuthorizationException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateClaimException"/> class.
        /// </summary>
        /// <param name="claimType">Type of the claim.</param>
        public DuplicateClaimException(string claimType)
            : base($"Claim '{claimType}' is found multiple times, while it's only supported once.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateClaimException"/> class.
        /// </summary>
        public DuplicateClaimException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateClaimException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public DuplicateClaimException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
