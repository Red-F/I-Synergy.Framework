﻿using ISynergy.Framework.Payment.Mollie.Enumerations;

namespace ISynergy.Framework.Payment.Mollie.Models.Payment.Request
{
    /// <summary>
    /// Class SepaDirectDebitRequest.
    /// Implements the <see cref="PaymentRequest" />
    /// </summary>
    /// <seealso cref="PaymentRequest" />
    public class SepaDirectDebitRequest : PaymentRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SepaDirectDebitRequest" /> class.
        /// </summary>
        public SepaDirectDebitRequest()
        {
            Method = PaymentMethods.DirectDebit;
        }

        /// <summary>
        /// Optional - Beneficiary name of the account holder.
        /// </summary>
        /// <value>The name of the consumer.</value>
        public string ConsumerName { get; set; }

        /// <summary>
        /// Optional - IBAN of the account holder.
        /// </summary>
        /// <value>The consumer account.</value>
        public string ConsumerAccount { get; set; }
    }
}
