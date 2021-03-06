﻿using ISynergy.Framework.Payment.Mollie.Models.Payment.Response;
using ISynergy.Framework.Payment.Mollie.Services;
using Xunit;

namespace ISynergy.Framework.Payment.Mollie.Tests.Services
{
    /// <summary>
    /// Class JsonConverterServiceTests.
    /// </summary>
    /// <autogeneratedoc />
    public class JsonConverterServiceTests
    {
        /// <summary>
        /// Defines the test method CanDeserializeJsonMetadata.
        /// </summary>
        /// <autogeneratedoc />
        [Fact]
        public void CanDeserializeJsonMetadata()
        {
            // Given: A JSON metadata value
            var jsonConverterService = new JsonConverterService();
            var metadataJson = @"{
  ""ReferenceNumber"": null,
  ""OrderID"": null,
  ""UserID"": ""534721""
}";
            var paymentJson = @"{""metadata"":" + metadataJson + "}";

            // When: We deserialize the JSON
            var payments = jsonConverterService.Deserialize<PaymentResponse>(paymentJson);

            // Then: 
            Assert.Equal(metadataJson, payments.Metadata);
        }

        /// <summary>
        /// Defines the test method CanDeserializeStringMetadataValue.
        /// </summary>
        /// <autogeneratedoc />
        [Fact]
        public void CanDeserializeStringMetadataValue()
        {
            // Given: A JSON metadata value
            var jsonConverterService = new JsonConverterService();
            var metadataJson = "This is my metadata";
            var paymentJson = @"{""metadata"":""" + metadataJson + @"""}";

            // When: We deserialize the JSON
            var payments = jsonConverterService.Deserialize<PaymentResponse>(paymentJson);

            // Then: 
            Assert.Equal(metadataJson, payments.Metadata);
        }

        /// <summary>
        /// Defines the test method CanDeserializeNullMetadataValue.
        /// </summary>
        /// <autogeneratedoc />
        [Fact]
        public void CanDeserializeNullMetadataValue()
        {
            // Given: A JSON metadata value
            var jsonConverterService = new JsonConverterService();
            var metadataJson = @"null";
            var paymentJson = @"{""metadata"":" + metadataJson + "}";

            // When: We deserialize the JSON
            var payments = jsonConverterService.Deserialize<PaymentResponse>(paymentJson);

            // Then: 
            Assert.Null(payments.Metadata);
        }
    }
}
