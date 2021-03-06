﻿using ISynergy.Framework.Payment.Mollie.Models.Customer;
using ISynergy.Framework.Payment.Mollie.Models.Url;

namespace ISynergy.Framework.Payment.Mollie.Models.Subscription
{
    /// <summary>
    /// Class SubscriptionResponseLinks.
    /// </summary>
    public class SubscriptionResponseLinks
    {
        /// <summary>
        /// The API resource URL of the subscription itself.
        /// </summary>
        /// <value>The self.</value>
        public UrlObjectLink<SubscriptionResponse> Self { get; set; }

        /// <summary>
        /// The API resource URL of the customer the subscription is for.
        /// </summary>
        /// <value>The customer.</value>
        public UrlObjectLink<CustomerResponse> Customer { get; set; }

        /// <summary>
        /// The URL to the subscription retrieval endpoint documentation.
        /// </summary>
        /// <value>The documentation.</value>
        public UrlLink Documentation { get; set; }
    }
}
