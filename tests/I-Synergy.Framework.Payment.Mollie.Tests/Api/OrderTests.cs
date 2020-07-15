﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ISynergy.Framework.Payment.Mollie.Enumerations;
using ISynergy.Framework.Payment.Mollie.Models;
using ISynergy.Framework.Payment.Mollie.Models.Order;
using ISynergy.Framework.Payment.Mollie.Models.Payment;
using Xunit;

namespace ISynergy.Framework.Payment.Mollie.Tests.Api
{
    /// <summary>
    /// Class OrderTests.
    /// Implements the <see cref="BaseApiTestFixture" />
    /// </summary>
    /// <seealso cref="BaseApiTestFixture" />
    /// <autogeneratedoc />
    public class OrderTests : BaseApiTestFixture
    {
        /// <summary>
        /// Defines the test method CanCreateOrderWithOnlyRequiredFields.
        /// </summary>
        /// <autogeneratedoc />
        [Fact]
        public async Task CanCreateOrderWithOnlyRequiredFields() {
            // If: we create a order request with only the required parameters
            var orderRequest = CreateOrderRequestWithOnlyRequiredFields();

            // When: We send the order request to Mollie
            var result = await OrderClient.CreateOrderAsync(orderRequest);

            // Then: Make sure we get a valid response
            Assert.NotNull(result);
            Assert.Equal(orderRequest.Amount.Value, result.Amount.Value);
            Assert.Equal(orderRequest.Amount.Currency, result.Amount.Currency);
            Assert.Equal(orderRequest.OrderNumber, result.OrderNumber);
        }

        /// <summary>
        /// Defines the test method CanRetrieveOrderAfterCreationOrder.
        /// </summary>
        /// <autogeneratedoc />
        [Fact]
        public async Task CanRetrieveOrderAfterCreationOrder() {
            // If: we create a new order
            var orderRequest = CreateOrderRequestWithOnlyRequiredFields();
            var createdOrder = await OrderClient.CreateOrderAsync(orderRequest);

            // When: We attempt to retrieve the order
            var retrievedOrder = await OrderClient.GetOrderAsync(createdOrder.Id);

            // Then: Make sure we get a valid response
            Assert.NotNull(retrievedOrder);
            Assert.Equal(createdOrder.Id, retrievedOrder.Id);
        }

        /// <summary>
        /// Defines the test method CanUpdateExistingOrder.
        /// </summary>
        /// <autogeneratedoc />
        [Fact]
        public async Task CanUpdateExistingOrder() {
            // If: we create a new order
            var orderRequest = CreateOrderRequestWithOnlyRequiredFields();
            var createdOrder = await OrderClient.CreateOrderAsync(orderRequest);

            // When: We attempt to update the order
            var orderUpdateRequest = new OrderUpdateRequest() {
                OrderNumber = "1337",
                BillingAddress = createdOrder.BillingAddress
            };
            orderUpdateRequest.BillingAddress.City = "Den Haag";
            var updatedOrder = await OrderClient.UpdateOrderAsync(createdOrder.Id, orderUpdateRequest);

            // Then: Make sure the order is updated
            Assert.Equal(orderUpdateRequest.OrderNumber, updatedOrder.OrderNumber);
            Assert.Equal(orderUpdateRequest.BillingAddress.City, updatedOrder.BillingAddress.City);
        }

        /// <summary>
        /// Defines the test method CanCancelCreatedOrder.
        /// </summary>
        /// <autogeneratedoc />
        [Fact]
        public async Task CanCancelCreatedOrder() {
            // If: we create a new order
            var orderRequest = CreateOrderRequestWithOnlyRequiredFields();
            var createdOrder = await OrderClient.CreateOrderAsync(orderRequest);

            // When: We attempt to cancel the order and then retrieve it
            await OrderClient.CancelOrderAsync(createdOrder.Id);
            var canceledOrder = await OrderClient.GetOrderAsync(createdOrder.Id);

            // Then: The order status should be cancelled
            Assert.Equal(OrderStatus.Canceled, canceledOrder.Status);
        }

        /// <summary>
        /// Defines the test method CanUpdateOrderLine.
        /// </summary>
        /// <autogeneratedoc />
        [Fact]
        public async Task CanUpdateOrderLine() {
            // If: we create a new order
            var orderRequest = CreateOrderRequestWithOnlyRequiredFields();
            var createdOrder = await OrderClient.CreateOrderAsync(orderRequest);

            // When: We update the order line
            var updateRequest = new OrderLineUpdateRequest() {
                Name = "A fluffy bear"
            };
            var updatedOrder = await OrderClient.UpdateOrderLinesAsync(createdOrder.Id, createdOrder.Lines.First().Id, updateRequest);

            // Then: The name of the order line should be updated
            Assert.Equal(updateRequest.Name, updatedOrder.Lines.First().Name);
        }

        /// <summary>
        /// Defines the test method CanRetrieveOrderList.
        /// </summary>
        /// <autogeneratedoc />
        [Fact]
        public async Task CanRetrieveOrderList() {
            // When: Retrieve payment list with default settings
            var response = await OrderClient.GetOrderListAsync();

            // Then
            Assert.NotNull(response);
            Assert.NotNull(response.Items);
        }

        /// <summary>
        /// Defines the test method ListOrdersNeverReturnsMorePaymentsThenTheNumberOfRequestedOrders.
        /// </summary>
        /// <autogeneratedoc />
        [Fact]
        public async Task ListOrdersNeverReturnsMorePaymentsThenTheNumberOfRequestedOrders() {
            // If: Number of orders requested is 5
            var numberOfOrders = 5;

            // When: Retrieve 5 orders
            var response = await OrderClient.GetOrderListAsync(null, numberOfOrders);

            // Then
            Assert.True(response.Items.Count <= numberOfOrders);
        }

        /// <summary>
        /// Creates the order request with only required fields.
        /// </summary>
        /// <returns>OrderRequest.</returns>
        /// <autogeneratedoc />
        private OrderRequest CreateOrderRequestWithOnlyRequiredFields() {
            return new OrderRequest() {
                Amount = new Amount(Currency.EUR, "100.00"),
                OrderNumber = "16738",
                Lines = new List<OrderLineRequest>() {
                    new OrderLineRequest() {
                        Name = "A box of chocolates",
                        Quantity = 1,
                        UnitPrice = new Amount(Currency.EUR, "100.00"),
                        TotalAmount = new Amount(Currency.EUR, "100.00"),
                        VatRate = "21.00",
                        VatAmount = new Amount(Currency.EUR, "17.36")
                    }
                },
                BillingAddress = new OrderAddressDetails() {
                    GivenName = "John",
                    FamilyName = "Smit",
                    Email = "johnsmit@gmail.com",
                    City = "Rotterdam",
                    Country = "NL",
                    PostalCode = "0000AA",
                    Region = "Zuid-Holland",
                    StreetAndNumber = "Coolsingel 1"
                },
                RedirectUrl = "http://www.google.nl",
                Locale = Locale.nl_NL
            };
        }
    }
}
