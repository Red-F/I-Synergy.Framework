﻿using System.Diagnostics;
using System.Threading.Tasks;
using ISynergy.Framework.Payment.Mollie.Models;
using ISynergy.Framework.Payment.Mollie.Models.Payment.Request;
using ISynergy.Framework.Payment.Mollie.Models.Payment.Response;
using ISynergy.Framework.Payment.Mollie.Models.Refund;
using Xunit;

namespace ISynergy.Framework.Payment.Mollie.Tests.Api
{
    /// <summary>
    /// Class RefundTests.
    /// Implements the <see cref="BaseApiTestFixture" />
    /// </summary>
    /// <seealso cref="BaseApiTestFixture" />
    /// <autogeneratedoc />
    public class RefundTests : BaseApiTestFixture
    {
        /// <summary>
        /// Defines the test method CanCreateRefund.
        /// </summary>
        /// <autogeneratedoc />
        [Fact(Skip = "We can only test this in debug mode, because we actually have to use the PaymentUrl to make the payment, since Mollie can only refund payments that have been paid")]
        public async Task CanCreateRefund() {
            // If: We create a payment
            var amount = "100.00";
            var payment = await CreatePayment(amount);

            // We can only test this if you make the payment using the payment.Links.Checkout property. 
            // If you don't do this, this test will fail because we can only refund payments that have been paid
            Debugger.Break(); 

            // When: We attempt to refund this payment
            var refundRequest = new RefundRequest() {
                Amount = new Amount(Currency.EUR, amount)
            };
            var refundResponse = await RefundClient.CreateRefundAsync(payment.Id, refundRequest);

            // Then
            Assert.NotNull(refundResponse);
        }

        /// <summary>
        /// Defines the test method CanCreatePartialRefund.
        /// </summary>
        /// <autogeneratedoc />
        [Fact(Skip = "We can only test this in debug mode, because we actually have to use the PaymentUrl to make the payment, since Mollie can only refund payments that have been paid")]
        public async Task CanCreatePartialRefund() {
            // If: We create a payment of 250 euro
            var payment = await CreatePayment("250.00");

            // We can only test this if you make the payment using the payment.Links.PaymentUrl property. 
            // If you don't do this, this test will fail because we can only refund payments that have been paid
            Debugger.Break();

            // When: We attempt to refund 50 euro
            var refundRequest = new RefundRequest() {
                Amount = new Amount(Currency.EUR, "50.00")
            };
            var refundResponse = await RefundClient.CreateRefundAsync(payment.Id, refundRequest);

            // Then
            Assert.Equal("50.00", refundResponse.Amount.Value);
        }

        /// <summary>
        /// Defines the test method CanRetrieveSingleRefund.
        /// </summary>
        /// <autogeneratedoc />
        [Fact(Skip = "We can only test this in debug mode, because we actually have to use the PaymentUrl to make the payment, since Mollie can only refund payments that have been paid")]
        public async Task CanRetrieveSingleRefund() {
            // If: We create a payment
            var payment = await CreatePayment();
            // We can only test this if you make the payment using the payment.Links.PaymentUrl property. 
            // If you don't do this, this test will fail because we can only refund payments that have been paid
            Debugger.Break();

            var refundRequest = new RefundRequest() {
                Amount = new Amount(Currency.EUR, "50.00")
            };
            var refundResponse = await RefundClient.CreateRefundAsync(payment.Id, refundRequest);

            // When: We attempt to retrieve this refund
            var result = await RefundClient.GetRefundAsync(payment.Id, refundResponse.Id);

            // Then
            Assert.NotNull(result);
            Assert.Equal(refundResponse.Id, result.Id);
            Assert.Equal(refundResponse.Amount.Value, result.Amount.Value);
            Assert.Equal(refundResponse.Amount.Currency, result.Amount.Currency);
        }

        /// <summary>
        /// Defines the test method CanRetrieveRefundList.
        /// </summary>
        /// <autogeneratedoc />
        [Fact(Skip ="Outcome depends on payment methods active in portal")]
        public async Task CanRetrieveRefundList() {
            // If: We create a payment
            var payment = await CreatePayment();

            // When: Retrieve refund list for this payment
            var refundList = await RefundClient.GetRefundListAsync(payment.Id);

            // Then
            Assert.NotNull(refundList);
            Assert.NotNull(refundList.Items);
        }

        /// <summary>
        /// Creates the payment.
        /// </summary>
        /// <param name="amount">The amount.</param>
        /// <returns>PaymentResponse.</returns>
        /// <autogeneratedoc />
        private async Task<PaymentResponse> CreatePayment(string amount = "100.00") {
            var paymentRequest = new CreditCardPaymentRequest
            {
                Amount = new Amount(Currency.EUR, amount),
                Description = "Description",
                RedirectUrl = DefaultRedirectUrl
            };

            return await PaymentClient.CreatePaymentAsync(paymentRequest);
        }
    }
}
