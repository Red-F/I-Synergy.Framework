﻿using ISynergy.Framework.Payment.Mollie.Enumerations;

namespace ISynergy.Framework.Payment.Mollie.Models.Settlement
{
    /// <summary>
    /// Class SettlementPeriodCosts.
    /// </summary>
    public class SettlementPeriodCosts
    {
        /// <summary>
        /// A description of the subtotal.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }

        /// <summary>
        /// The net total of received funds for this payment method (excludes VAT).
        /// </summary>
        /// <value>The amount net.</value>
        public Amount AmountNet { get; set; }

        /// <summary>
        /// The VAT amount applicable to the revenue.
        /// </summary>
        /// <value>The amount vat.</value>
        public Amount AmountVat { get; set; }

        /// <summary>
        /// The gross total of received funds for this payment method (includes VAT).
        /// </summary>
        /// <value>The amount gross.</value>
        public Amount AmountGross { get; set; }

        /// <summary>
        /// The number of payments received for this payment method.
        /// </summary>
        /// <value>The count.</value>
        public int Count { get; set; }

        /// <summary>
        /// The service rates, further divided into fixed and variable costs.
        /// </summary>
        /// <value>The rate.</value>
        public SettlementPeriodCostsRate Rate { get; set; }

        /// <summary>
        /// The payment method ID, if applicable.
        /// </summary>
        /// <value>The method.</value>
        public PaymentMethods? Method { get; set; }
    }
}
