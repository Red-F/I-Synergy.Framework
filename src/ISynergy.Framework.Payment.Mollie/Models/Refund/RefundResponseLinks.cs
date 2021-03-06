﻿using ISynergy.Framework.Payment.Mollie.Models.Payment.Response;
using ISynergy.Framework.Payment.Mollie.Models.Settlement;
using ISynergy.Framework.Payment.Mollie.Models.Url;

namespace ISynergy.Framework.Payment.Mollie.Models.Refund
{
    /// <summary>
    /// Class RefundResponseLinks.
    /// </summary>
    public class RefundResponseLinks
    {
        /// <summary>
        /// The API resource URL of the refund itself.
        /// </summary>
        /// <value>The self.</value>
        public UrlObjectLink<RefundResponse> Self { get; set; }

        /// <summary>
        /// The API resource URL of the payment the refund belongs to.
        /// </summary>
        /// <value>The payment.</value>
        public UrlObjectLink<PaymentResponse> Payment { get; set; }

        /// <summary>
        /// The API resource URL of the settlement this payment has been settled with. Not present if not yet settled.
        /// </summary>
        /// <value>The settlement.</value>
        public UrlObjectLink<SettlementResponse> Settlement { get; set; }

        /// <summary>
        /// The URL to the refund retrieval endpoint documentation.
        /// </summary>
        /// <value>The documentation.</value>
        public UrlLink Documentation { get; set; }
    }
}
