﻿namespace ISynergy.Framework.Payment.Mollie.Models.Payment.Request
{
    /// <summary>
    /// Class PaymentRequestApplicationFee.
    /// </summary>
    public class PaymentRequestApplicationFee
    {
        /// <summary>
        /// The amount in EURO that the app wants to charge, e.g. 10.00 if the app would want to charge €10.00.
        /// </summary>
        /// <value>The amount.</value>
        public Amount Amount { get; set; }

        /// <summary>
        /// The description of the application fee. This will appear on settlement reports to the merchant and to you.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }
    }
}
