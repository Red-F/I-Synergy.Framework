﻿using System;

namespace ISynergy.Framework.AspNetCore.Authentication.Exceptions
{
    public class ClaimNotFoundException : ClaimAuthorizationException
    {
        public ClaimNotFoundException()
        {
        }

        public ClaimNotFoundException(string claimType) : base($"Claim '{claimType}' not found.")
        {
        }

        public ClaimNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}