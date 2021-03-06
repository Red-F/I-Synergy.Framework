﻿namespace ISynergy.Framework.Payment.Mollie.Models
{
    /// <summary>
    /// Class AddressObject.
    /// </summary>
    public class AddressObject
    {
        /// <summary>
        /// The card holder’s street and street number.
        /// </summary>
        /// <value>The street and number.</value>
        public string StreetAndNumber { get; set; }

        /// <summary>
        /// The card holder’s postal code.
        /// </summary>
        /// <value>The postal code.</value>
        public string PostalCode { get; set; }

        /// <summary>
        /// The card holder’s city.
        /// </summary>
        /// <value>The city.</value>
        public string City { get; set; }

        /// <summary>
        /// The card holder’s region.
        /// </summary>
        /// <value>The region.</value>
        public string Region { get; set; }

        /// <summary>
        /// The card holder’s country in ISO 3166-1 alpha-2 format.
        /// </summary>
        /// <value>The country.</value>
        public string Country { get; set; }
    }
}
