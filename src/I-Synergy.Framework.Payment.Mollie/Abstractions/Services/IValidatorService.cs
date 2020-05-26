﻿using ISynergy.Framework.Payment.Mollie.Models.Url;

namespace ISynergy.Framework.Payment.Mollie.Abstractions.Services
{
    /// <summary>
    /// Interface IValidatorService
    /// </summary>
    /// <autogeneratedoc />
    public interface IValidatorService
    {
        /// <summary>
        /// Validates the URL link.
        /// </summary>
        /// <param name="urlObject">The URL object.</param>
        /// <autogeneratedoc />
        void ValidateUrlLink(UrlLink urlObject);
    }
}
