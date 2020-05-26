﻿using System;
using System.Threading.Tasks;
using System.Linq;
using Xunit;
using ISynergy.Framework.Payment.Mollie.Models.Payment.Request;
using ISynergy.Framework.Payment.Mollie.Models;
using ISynergy.Framework.Payment.Mollie.Models.Payment.Response;
using ISynergy.Framework.Payment.Mollie.Models.Payment;
using ISynergy.Framework.Payment.Mollie.Enumerations;
using ISynergy.Framework.Payment.Mollie.Models.Payment.Response.Specific;
using ISynergy.Framework.Payment.Mollie.Models.Mandate;

namespace ISynergy.Framework.Payment.Mollie.Tests.Api
{
    /// <summary>
    /// Class PaymentTests.
    /// Implements the <see cref="ISynergy.Framework.Payment.Mollie.Tests.Api.BaseApiTestFixture" />
    /// </summary>
    /// <seealso cref="ISynergy.Framework.Payment.Mollie.Tests.Api.BaseApiTestFixture" />
    /// <autogeneratedoc />
    public class PaymentTests : BaseApiTestFixture
    {
        /// <summary>
        /// Defines the test method CanRetrievePaymentList.
        /// </summary>
        /// <autogeneratedoc />
        [Fact]
        public async Task CanRetrievePaymentList() {
            // When: Retrieve payment list with default settings
            var response = await PaymentClient.GetPaymentListAsync();

            // Then
            Assert.NotNull(response);
            Assert.NotNull(response.Items);
        }

        /// <summary>
        /// Defines the test method ListPaymentsNeverReturnsMorePaymentsThenTheNumberOfRequestedPayments.
        /// </summary>
        /// <autogeneratedoc />
        [Fact]
        public async Task ListPaymentsNeverReturnsMorePaymentsThenTheNumberOfRequestedPayments() {
            // When: Number of payments requested is 5
            var numberOfPayments = 5;

            // When: Retrieve 5 payments
            var response = await PaymentClient.GetPaymentListAsync(null, numberOfPayments);

            // Then
            Assert.True(response.Items.Count <= numberOfPayments);
        }

        /// <summary>
        /// Defines the test method WhenRetrievingAListOfPaymentsPaymentSubclassesShouldBeInitialized.
        /// </summary>
        /// <autogeneratedoc />
        [Fact]
        public async Task WhenRetrievingAListOfPaymentsPaymentSubclassesShouldBeInitialized() {
            // Given: We create a new payment 
            var paymentRequest = new IdealPaymentRequest() {
                Amount = new Amount(Currency.EUR, "100.00"),
                Description = "Description",
                RedirectUrl = DefaultRedirectUrl
            };
            await PaymentClient.CreatePaymentAsync(paymentRequest);

            // When: We retrieve it in a list
            var result = await PaymentClient.GetPaymentListAsync(null, 5);

            // Then: We expect a list with a single ideal payment            
            Assert.IsAssignableFrom<IdealPaymentResponse>(result.Items.First());
        }

        /// <summary>
        /// Defines the test method CanCreateDefaultPaymentWithOnlyRequiredFields.
        /// </summary>
        /// <autogeneratedoc />
        [Fact]
        public async Task CanCreateDefaultPaymentWithOnlyRequiredFields() {
            // When: we create a payment request with only the required parameters
            var paymentRequest = new PaymentRequest() {
                Amount = new Amount(Currency.EUR, "100.00"),
                Description = "Description",
                RedirectUrl = DefaultRedirectUrl
            };

            // When: We send the payment request to Mollie
            var result = await PaymentClient.CreatePaymentAsync(paymentRequest);

            // Then: Make sure we get a valid response
            Assert.NotNull(result);
            Assert.Equal(paymentRequest.Amount.Currency, result.Amount.Currency);
            Assert.Equal(paymentRequest.Amount.Value, result.Amount.Value);
            Assert.Equal(paymentRequest.Description, result.Description);
            Assert.Equal(paymentRequest.RedirectUrl, result.RedirectUrl);
        }

        /// <summary>
        /// Defines the test method CanCreateDefaultPaymentWithAllFields.
        /// </summary>
        /// <autogeneratedoc />
        [Fact]
        public async Task CanCreateDefaultPaymentWithAllFields() {
            // If: we create a payment request where all parameters have a value
            var paymentRequest = new PaymentRequest() {
                Amount = new Amount(Currency.EUR, "100.00"),
                Description = "Description",
                RedirectUrl = DefaultRedirectUrl,
                Locale = Locale.nl_NL,
                Metadata = "{\"firstName\":\"John\",\"lastName\":\"Doe\"}",
                Method = PaymentMethods.BankTransfer,
                WebhookUrl = DefaultWebhookUrl
            };

            // When: We send the payment request to Mollie
            var result = await PaymentClient.CreatePaymentAsync(paymentRequest);

            // Then: Make sure all requested parameters match the response parameter values
            Assert.NotNull(result);
            Assert.Equal(paymentRequest.Amount.Currency, result.Amount.Currency);
            Assert.Equal(paymentRequest.Amount.Value, result.Amount.Value);
            Assert.Equal(paymentRequest.Description, result.Description);
            Assert.Equal(paymentRequest.RedirectUrl, result.RedirectUrl);
            Assert.Equal(paymentRequest.Locale, result.Locale);
            Assert.Equal(paymentRequest.Metadata, result.Metadata);
            Assert.Equal(paymentRequest.WebhookUrl, result.WebhookUrl);
        }

        /// <summary>
        /// Defines the test method CanCreateSpecificPaymentType.
        /// </summary>
        /// <param name="paymentType">Type of the payment.</param>
        /// <param name="paymentMethod">The payment method.</param>
        /// <param name="expectedResponseType">Expected type of the response.</param>
        /// <autogeneratedoc />
        [Theory(Skip = "Outcome depends on payment methods active in portal")]
        [InlineData(typeof(IdealPaymentRequest), PaymentMethods.Ideal, typeof(IdealPaymentResponse))]
        [InlineData(typeof(CreditCardPaymentRequest), PaymentMethods.CreditCard, typeof(CreditCardPaymentResponse))]
        [InlineData(typeof(PaymentRequest), PaymentMethods.Bancontact, typeof(BancontactPaymentResponse))]
        [InlineData(typeof(PaymentRequest), PaymentMethods.Sofort, typeof(SofortPaymentResponse))]
        [InlineData(typeof(BankTransferPaymentRequest), PaymentMethods.BankTransfer, typeof(BankTransferPaymentResponse))]
        [InlineData(typeof(PayPalPaymentRequest), PaymentMethods.PayPal, typeof(PayPalPaymentResponse))]
        [InlineData(typeof(BitcoinPaymentRequest), PaymentMethods.Bitcoin, typeof(BitcoinPaymentResponse))]
        [InlineData(typeof(PaymentRequest), PaymentMethods.Belfius, typeof(BelfiusPaymentResponse))]
        [InlineData(typeof(KbcPaymentRequest), PaymentMethods.Kbc, typeof(KbcPaymentResponse))]
        [InlineData(typeof(PaymentRequest), null, typeof(PaymentResponse))]
        //[TestCase(typeof(Przelewy24PaymentRequest), PaymentMethod.Przelewy24, typeof(PaymentResponse))] // Payment option is not enabled in website profile
        public async Task CanCreateSpecificPaymentType(Type paymentType, PaymentMethods? paymentMethod, Type expectedResponseType) {
            // If: we create a specific payment type with some bank transfer specific values
            var paymentRequest = (PaymentRequest) Activator.CreateInstance(paymentType);
            paymentRequest.Amount = new Amount(Currency.EUR, "100.00");
            paymentRequest.Description = "Description";
            paymentRequest.RedirectUrl = DefaultRedirectUrl;
            paymentRequest.Method = paymentMethod;

            // Set required billing email for Przelewy24
            if (paymentRequest is Przelewy24PaymentRequest request)
            {
                request.BillingEmail = "example@example.com";
            }

            // When: We send the payment request to Mollie
            var result = await PaymentClient.CreatePaymentAsync(paymentRequest);

            // Then: Make sure all requested parameters match the response parameter values
            Assert.NotNull(result);
            Assert.Equal(expectedResponseType, result.GetType());
            Assert.Equal(paymentRequest.Amount.Currency, result.Amount.Currency);
            Assert.Equal(paymentRequest.Amount.Value, result.Amount.Value);
            Assert.Equal(paymentRequest.Description, result.Description);
            Assert.Equal(paymentRequest.RedirectUrl, result.RedirectUrl);
        }

        /// <summary>
        /// Defines the test method CanCreatePaymentAndRetrieveIt.
        /// </summary>
        /// <autogeneratedoc />
        [Fact]
        public async Task CanCreatePaymentAndRetrieveIt() {
            // If: we create a new payment request
            var paymentRequest = new PaymentRequest() {
                Amount = new Amount(Currency.EUR, "100.00"),
                Description = "Description",
                RedirectUrl = DefaultRedirectUrl,
                Locale = Locale.de_DE
            };

            // When: We send the payment request to Mollie and attempt to retrieve it
            var paymentResponse = await PaymentClient.CreatePaymentAsync(paymentRequest);
            var result = await PaymentClient.GetPaymentAsync(paymentResponse.Id);

            // Then
            Assert.NotNull(result);
            Assert.Equal(paymentResponse.Id, result.Id);
            Assert.Equal(paymentResponse.Amount.Currency, result.Amount.Currency);
            Assert.Equal(paymentResponse.Amount.Value, result.Amount.Value);
            Assert.Equal(paymentResponse.Description, result.Description);
            Assert.Equal(paymentResponse.RedirectUrl, result.RedirectUrl);
        }

        /// <summary>
        /// Defines the test method CanCreateRecurringPaymentAndRetrieveIt.
        /// </summary>
        /// <autogeneratedoc />
        [Fact]
        public async Task CanCreateRecurringPaymentAndRetrieveIt() {
            // If: we create a new recurring payment
            var mandate = await GetFirstValidMandate();
            var customer = await CustomerClient.GetCustomerAsync(mandate.Links.Customer);
            var paymentRequest = new PaymentRequest() {
                Amount = new Amount(Currency.EUR, "100.00"),
                Description = "Description",
                RedirectUrl = DefaultRedirectUrl,
                SequenceType = SequenceType.First,
                CustomerId = customer.Id
            };

            // When: We send the payment request to Mollie and attempt to retrieve it
            var paymentResponse = await PaymentClient.CreatePaymentAsync(paymentRequest);
            var result = await PaymentClient.GetPaymentAsync(paymentResponse.Id);

            // Then: Make sure the recurringtype parameter is entered
            Assert.Equal(SequenceType.First, result.SequenceType);
        }

        /// <summary>
        /// Defines the test method CanCreatePaymentWithMetaData.
        /// </summary>
        /// <autogeneratedoc />
        [Fact]
        public async Task CanCreatePaymentWithMetaData() {
            // If: We create a payment with meta data
            var metadata = "this is my metadata";
            var paymentRequest = new PaymentRequest() {
                Amount = new Amount(Currency.EUR, "100.00"),
                Description = "Description",
                RedirectUrl = DefaultRedirectUrl,
                Metadata = metadata
            };

            // When: We send the payment request to Mollie
            var result = await PaymentClient.CreatePaymentAsync(paymentRequest);

            // Then: Make sure we get the same json result as metadata
            Assert.Equal(metadata, result.Metadata);
        }

        /// <summary>
        /// Defines the test method CanCreatePaymentWithJsonMetaData.
        /// </summary>
        /// <autogeneratedoc />
        [Fact]
        public async Task CanCreatePaymentWithJsonMetaData() {
            // If: We create a payment with meta data
            var json = "{\"order_id\":\"4.40\"}";
            var paymentRequest = new PaymentRequest() {
                Amount = new Amount(Currency.EUR, "100.00"),
                Description = "Description",
                RedirectUrl = DefaultRedirectUrl,
                Metadata = json
            };

            // When: We send the payment request to Mollie
            var result = await PaymentClient.CreatePaymentAsync(paymentRequest);

            // Then: Make sure we get the same json result as metadata
            Assert.Equal(json, result.Metadata);
        }

        /// <summary>
        /// Defines the test method CanCreatePaymentWithCustomMetaDataClass.
        /// </summary>
        /// <autogeneratedoc />
        [Fact]
        public async Task CanCreatePaymentWithCustomMetaDataClass() {
            // If: We create a payment with meta data
            var metadataRequest = new CustomMetadataClass() {
                OrderId = 1,
                Description = "Custom description"
            };

            var paymentRequest = new PaymentRequest() {
                Amount = new Amount(Currency.EUR, "100.00"),
                Description = "Description",
                RedirectUrl = DefaultRedirectUrl,
            };
            paymentRequest.SetMetadata(metadataRequest);

            // When: We send the payment request to Mollie
            var result = await PaymentClient.CreatePaymentAsync(paymentRequest);
            var metadataResponse = result.GetMetadata<CustomMetadataClass>();

            // Then: Make sure we get the same json result as metadata
            Assert.NotNull(metadataResponse);
            Assert.Equal(metadataRequest.OrderId, metadataResponse.OrderId);
            Assert.Equal(metadataRequest.Description, metadataResponse.Description);
        }

        /// <summary>
        /// Defines the test method CanCreatePaymentWithMandate.
        /// </summary>
        /// <autogeneratedoc />
        [Fact]
        public async Task CanCreatePaymentWithMandate() {
            // If: We create a payment with a mandate id
            var validMandate = await GetFirstValidMandate();
            var customer = await CustomerClient.GetCustomerAsync(validMandate.Links.Customer);
            var paymentRequest = new PaymentRequest() {
                Amount = new Amount(Currency.EUR, "100.00"),
                Description = "Description",
                RedirectUrl = DefaultRedirectUrl,
                SequenceType = SequenceType.Recurring,
                CustomerId = customer.Id,
                MandateId = validMandate.Id
            };

            // When: We send the payment request to Mollie
            var result = await PaymentClient.CreatePaymentAsync(paymentRequest);

            // Then: Make sure we get the mandate id back in the details
            Assert.Equal(validMandate.Id, result.MandateId);
        }

        //[Fact]
        //public async Task PaymentWithDifferentHttpInstance() {
        //    // If: We create a PaymentClient with our own HttpClient instance
        //    HttpClient myHttpClientInstance = new HttpClient();
        //    PaymentClient paymentClient = new PaymentClient(new ClientService(ApiTestKey, myHttpClientInstance));
        //    PaymentRequest paymentRequest = new PaymentRequest() {
        //        Amount = new Amount(Currency.EUR, "100.00"),
        //        Description = "Description",
        //        RedirectUrl = DefaultRedirectUrl
        //    };

        //    // When: I create a new payment
        //    PaymentResponse result = await paymentClient.CreatePaymentAsync(paymentRequest);

        //    // Then: It should still work... lol
        //    Assert.NotNull(result);
        //    Assert.Equal(paymentRequest.Amount.Currency, result.Amount.Currency);
        //    Assert.Equal(paymentRequest.Amount.Value, result.Amount.Value);
        //    Assert.Equal(paymentRequest.Description, result.Description);
        //    Assert.Equal(paymentRequest.RedirectUrl, result.RedirectUrl);
        //}

        /// <summary>
        /// Gets the first valid mandate.
        /// </summary>
        /// <returns>MandateResponse.</returns>
        /// <autogeneratedoc />
        private async Task<MandateResponse> GetFirstValidMandate() {
            var customers = await CustomerClient.GetCustomerListAsync();

            if (!customers.Items.Any()) {
                Assert.Empty(customers.Items);  //No customers found. Unable to test recurring payment tests
                return null;
            }

            foreach (var customer in customers.Items) {
                var customerMandates = await MandateClient.GetMandateListAsync(customer.Id);
                var firstValidMandate = customerMandates.Items.FirstOrDefault(x => x.Status == MandateStatus.Valid);
                if (firstValidMandate != null) {
                    return firstValidMandate;
                }
            }

            // No mandates found. Unable to test recurring payments
            return null;
        }
    }

    /// <summary>
    /// Class CustomMetadataClass.
    /// </summary>
    /// <autogeneratedoc />
    public class CustomMetadataClass {
        /// <summary>
        /// Gets or sets the order identifier.
        /// </summary>
        /// <value>The order identifier.</value>
        /// <autogeneratedoc />
        public int OrderId { get; set; }
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        /// <autogeneratedoc />
        public string Description { get; set; }
    }
}
