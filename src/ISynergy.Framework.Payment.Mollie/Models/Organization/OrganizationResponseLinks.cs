﻿using ISynergy.Framework.Payment.Mollie.Models.Url;

namespace ISynergy.Framework.Payment.Mollie.Models.Organization
{
    /// <summary>
    /// Class OrganizationResponseLinks.
    /// </summary>
    public class OrganizationResponseLinks
    {
        /// <summary>
        /// The API resource URL of the organization itself.
        /// </summary>
        /// <value>The self.</value>
        public UrlObjectLink<OrganizationResponse> Self { get; set; }

        /// <summary>
        /// The URL to the payment method retrieval endpoint documentation.
        /// </summary>
        /// <value>The documentation.</value>
        public UrlLink Documentation { get; set; }
    }
}
