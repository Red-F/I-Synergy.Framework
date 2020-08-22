﻿using System.ComponentModel.DataAnnotations;
using ISynergy.Framework.Payment.Mollie.Models;
using Mollie.Sample.Framework.Validators;

namespace Mollie.Sample.Models
{
    /// <summary>
    /// Class CreatePaymentModel.
    /// </summary>
    /// <autogeneratedoc />
    public class CreatePaymentModel {
        /// <summary>
        /// Gets or sets the amount.
        /// </summary>
        /// <value>The amount.</value>
        /// <autogeneratedoc />
        [Required]
        [Range(0.01, 1000, ErrorMessage = "Please enter an amount between 0.01 and 1000")]
        [RegularExpression(@"\d+(\.\d{2})?", ErrorMessage = "Please enter an amount with two decimal places")]
        public decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets the currency.
        /// </summary>
        /// <value>The currency.</value>
        /// <autogeneratedoc />
        [Required]
        [StaticStringList(typeof(Currency))]
        public string Currency { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        /// <autogeneratedoc />
        [Required]
        public string Description { get; set; }
    }
}
