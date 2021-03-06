﻿using ISynergy.Framework.Payment.Mollie.Models.Customer;
using ISynergy.Framework.Payment.Mollie.Models.Url;

namespace ISynergy.Framework.Payment.Mollie.Models.Mandate
{
    /// <summary>
    /// Class MandateResponseLinks.
    /// </summary>
    public class MandateResponseLinks
    {
        /// <summary>
        /// The API resource URL of the mandate itself.
        /// </summary>
        /// <value>The self.</value>
        public UrlObjectLink<MandateResponse> Self { get; set; }

        /// <summary>
        /// The API resource URL of the customer the mandate is for.
        /// </summary>
        /// <value>The customer.</value>
        public UrlObjectLink<CustomerResponse> Customer { get; set; }

        /// <summary>
        /// The URL to the mandate retrieval endpoint documentation.
        /// </summary>
        /// <value>The documentation.</value>
        public UrlLink Documentation { get; set; }
    }
}
