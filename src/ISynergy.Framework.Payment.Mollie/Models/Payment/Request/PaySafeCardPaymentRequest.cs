﻿using ISynergy.Framework.Payment.Mollie.Enumerations;

namespace ISynergy.Framework.Payment.Mollie.Models.Payment.Request
{
    /// <summary>
    /// Class PaySafeCardPaymentRequest.
    /// Implements the <see cref="PaymentRequest" />
    /// </summary>
    /// <seealso cref="PaymentRequest" />
    public class PaySafeCardPaymentRequest : PaymentRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PaySafeCardPaymentRequest" /> class.
        /// </summary>
        public PaySafeCardPaymentRequest()
        {
            Method = PaymentMethods.PaySafeCard;
        }

        /// <summary>
        /// Used for consumer identification. For example, you could use the consumer’s IP address.
        /// </summary>
        /// <value>The customer reference.</value>
        public string CustomerReference { get; set; }
    }
}
