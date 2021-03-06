﻿using System.ComponentModel.DataAnnotations;
using ISynergy.Framework.Payment.Mollie.Models;
using Mollie.Sample.Framework.Validators;

namespace Mollie.Sample.Models
{
    /// <summary>
    /// Class CreateSubscriptionModel.
    /// </summary>
    /// <autogeneratedoc />
    public class CreateSubscriptionModel {
        /// <summary>
        /// Gets or sets the customer identifier.
        /// </summary>
        /// <value>The customer identifier.</value>
        /// <autogeneratedoc />
        [Required]
        public string CustomerId { get; set; }

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
        /// Gets or sets the times.
        /// </summary>
        /// <value>The times.</value>
        /// <autogeneratedoc />
        [Range(1, 10)]
        public int? Times { get; set; }

        /// <summary>
        /// Gets or sets the interval amount.
        /// </summary>
        /// <value>The interval amount.</value>
        /// <autogeneratedoc />
        [Range(1, 20, ErrorMessage = "Please enter a interval number between 1 and 20")]
        [Required]
        [Display(Name = "Interval amount")]
        public int? IntervalAmount { get; set; }

        /// <summary>
        /// Gets or sets the interval period.
        /// </summary>
        /// <value>The interval period.</value>
        /// <autogeneratedoc />
        [Required]
        [Display(Name = "Interval period")]
        public IntervalPeriod IntervalPeriod { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        /// <autogeneratedoc />
        [Required]
        public string Description { get; set; }
    }

    /// <summary>
    /// Enum IntervalPeriod
    /// </summary>
    /// <autogeneratedoc />
    public enum IntervalPeriod {
        /// <summary>
        /// The months
        /// </summary>
        /// <autogeneratedoc />
        Months,
        /// <summary>
        /// The weeks
        /// </summary>
        /// <autogeneratedoc />
        Weeks,
        /// <summary>
        /// The days
        /// </summary>
        /// <autogeneratedoc />
        Days
    }
}
