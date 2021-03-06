﻿using ISynergy.Framework.Payment.Mollie.Abstractions.Models;

namespace ISynergy.Framework.Payment.Mollie.Models.Issuer
{
    /// <summary>
    /// Class IssuerResponse.
    /// Implements the <see cref="IResponseObject" />
    /// </summary>
    /// <seealso cref="IResponseObject" />
    public class IssuerResponse : IResponseObject
    {
        /// <summary>
        /// Contains "issuer"
        /// </summary>
        /// <value>The resource.</value>
        public string Resource { get; set; }

        /// <summary>
        /// The issuer's unique identifier, for example ideal_ABNANL2A. When creating a payment, specify this ID as the issuer
        /// parameter to forward
        /// the consumer to their banking environment directly.
        /// </summary>
        /// <value>The identifier.</value>
        public string Id { get; set; }

        /// <summary>
        /// The issuer's full name, for example 'ABN AMRO'.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Different Issuer Image icons (iDEAL).
        /// </summary>
        /// <value>The image.</value>
        public IssuerResponseImage Image { get; set; }
    }
}
