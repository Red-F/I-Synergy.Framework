﻿using ISynergy.Framework.Payment.Mollie.Enumerations;

namespace ISynergy.Framework.Payment.Mollie.Models.Payment.Request
{
    /// <summary>
    /// Class BitcoinPaymentRequest.
    /// Implements the <see cref="PaymentRequest" />
    /// </summary>
    /// <seealso cref="PaymentRequest" />
    public class BitcoinPaymentRequest : PaymentRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BitcoinPaymentRequest" /> class.
        /// </summary>
        public BitcoinPaymentRequest()
        {
            Method = PaymentMethods.Bitcoin;
        }

        /// <summary>
        /// The email address of the customer. This is used when handling invalid transactions (wrong amount
        /// transferred, transfer of expired or canceled payments, et cetera).
        /// </summary>
        /// <value>The billing email.</value>
        public string BillingEmail { get; set; }
    }
}
