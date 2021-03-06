﻿using ISynergy.Framework.Payment.Mollie.Models.Chargeback;
using ISynergy.Framework.Payment.Mollie.Models.Customer;
using ISynergy.Framework.Payment.Mollie.Models.List;
using ISynergy.Framework.Payment.Mollie.Models.Mandate;
using ISynergy.Framework.Payment.Mollie.Models.Settlement;
using ISynergy.Framework.Payment.Mollie.Models.Subscription;
using ISynergy.Framework.Payment.Mollie.Models.Url;

namespace ISynergy.Framework.Payment.Mollie.Models.Payment.Response
{
    /// <summary>
    /// Class PaymentResponseLinks.
    /// </summary>
    public class PaymentResponseLinks
    {
        /// <summary>
        /// The API resource URL of the payment itself.
        /// </summary>
        /// <value>The self.</value>
        public UrlObjectLink<PaymentResponse> Self { get; set; }

        /// <summary>
        /// The URL your customer should visit to make the payment. This is where you should redirect the consumer to.
        /// </summary>
        /// <value>The checkout.</value>
        public UrlLink Checkout { get; set; }

        /// <summary>
        /// The API resource URL of the refunds that belong to this payment.
        /// </summary>
        /// <value>The refunds.</value>
        public UrlLink Refunds { get; set; }

        /// <summary>
        /// The API resource URL of the chargebacks that belong to this payment.
        /// </summary>
        /// <value>The chargebacks.</value>
        public UrlObjectLink<ListResponse<ChargebackResponse>> Chargebacks { get; set; }

        /// <summary>
        /// The API resource URL of the settlement this payment has been settled with. Not present if not yet settled.
        /// </summary>
        /// <value>The settlement.</value>
        public UrlObjectLink<SettlementResponse> Settlement { get; set; }

        /// <summary>
        /// The URL to the payment retrieval endpoint documentation.
        /// </summary>
        /// <value>The documentation.</value>
        public UrlLink Documentation { get; set; }

        /// <summary>
        /// The API resource URL of the mandate linked to this payment. Not present if a one-off payment.
        /// </summary>
        /// <value>The mandate.</value>
        public UrlObjectLink<MandateResponse> Mandate { get; set; }

        /// <summary>
        /// The API resource URL of the subscription this payment is part of. Not present if not a subscription payment.
        /// </summary>
        /// <value>The subscription.</value>
        public UrlObjectLink<SubscriptionResponse> Subscription { get; set; }

        /// <summary>
        /// The API resource URL of the customer this payment belongs to. Not present if not linked to a customer.
        /// </summary>
        /// <value>The customer.</value>
        public UrlObjectLink<CustomerResponse> Customer { get; set; }
    }
}
