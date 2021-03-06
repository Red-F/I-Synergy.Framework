﻿using ISynergy.Framework.Payment.Mollie.Models.Chargeback;
using ISynergy.Framework.Payment.Mollie.Models.List;

using ISynergy.Framework.Payment.Mollie.Models.Payment.Response;
using ISynergy.Framework.Payment.Mollie.Models.Refund;
using ISynergy.Framework.Payment.Mollie.Models.Url;

namespace ISynergy.Framework.Payment.Mollie.Models.Settlement
{
    /// <summary>
    /// Class SettlementResponseLinks.
    /// </summary>
    public class SettlementResponseLinks
    {
        /// <summary>
        /// The API resource URL of the settlement itself.
        /// </summary>
        /// <value>The self.</value>
        public UrlObjectLink<SettlementResponse> Self { get; set; }

        /// <summary>
        /// The API resource URL of the payments that are included in this settlement.
        /// </summary>
        /// <value>The payments.</value>
        public UrlObjectLink<ListResponse<PaymentResponse>> Payments { get; set; }

        /// <summary>
        /// The API resource URL of the refunds that are included in this settlement.
        /// </summary>
        /// <value>The refunds.</value>
        public UrlObjectLink<ListResponse<RefundResponse>> Refunds { get; set; }

        /// <summary>
        /// The API resource URL of the chargebacks that are included in this settlement.
        /// </summary>
        /// <value>The chargebacks.</value>
        public UrlObjectLink<ListResponse<ChargebackResponse>> Chargebacks { get; set; }

        /// <summary>
        /// The URL to the settlement retrieval endpoint documentation.
        /// </summary>
        /// <value>The documentation.</value>
        public UrlLink Documentation { get; set; }
    }
}
