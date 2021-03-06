﻿namespace ISynergy.Framework.Payment.Mollie.Models.Refund
{
    /// <summary>
    /// Class RefundRequest.
    /// </summary>
    public class RefundRequest
    {
        /// <summary>
        /// The amount to refund. For some payments, it can be up to €25.00 more than the original transaction amount.
        /// </summary>
        /// <value>The amount.</value>
        public Amount Amount { get; set; }

        /// <summary>
        /// Optional – The description of the refund you are creating. This will be shown to the consumer on their card or bank
        /// statement when possible. Max 140 characters.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }

        /// <summary>
        /// Set this to true to refund a test mode payment.
        /// </summary>
        /// <value><c>null</c> if [testmode] contains no value, <c>true</c> if [testmode]; otherwise, <c>false</c>.</value>
        public bool? Testmode { get; set; }
    }
}
