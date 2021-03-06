﻿namespace ISynergy.Framework.Payment.Mollie.Models.Customer
{
    /// <summary>
    /// Class CustomerRequest.
    /// </summary>
    public class CustomerRequest
    {
        /// <summary>
        /// Required - The full name of the customer.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Required - The email address of the customer.
        /// </summary>
        /// <value>The email.</value>
        public string Email { get; set; }

        /// <summary>
        /// Allows you to preset the language to be used in the payment screens shown to the consumer. When this parameter is not
        /// provided, the browser language will be used instead in the payment flow (which is usually more accurate).
        /// </summary>
        /// <value>The locale.</value>
        public string Locale { get; set; }

        /// <summary>
        /// Optional - Provide any data you like in JSON notation, and we will save the data alongside the customer. Whenever
        /// you fetch the customer with our API, we'll also include the metadata. You can use up to 1kB of JSON.
        /// </summary>
        /// <value>The metadata.</value>
        public string Metadata { get; set; }

        /// <summary>
        /// Sets the metadata.
        /// </summary>
        /// <param name="metadataObj">The metadata object.</param>
        /// <param name="jsonSerializerSettings">The json serializer settings.</param>
        public void SetMetadata(object metadataObj, JsonSerializerSettings jsonSerializerSettings = null)
        {
            Metadata = JsonSerializer.Serialize(metadataObj, jsonSerializerSettings);
        }
    }
}
