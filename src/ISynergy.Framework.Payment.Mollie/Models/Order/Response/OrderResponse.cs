﻿using System;
using System.Collections.Generic;
using ISynergy.Framework.Payment.Converters;
using ISynergy.Framework.Payment.Mollie.Abstractions.Models;
using ISynergy.Framework.Payment.Mollie.Enumerations;

namespace ISynergy.Framework.Payment.Mollie.Models.Order
{
    /// <summary>
    /// Class OrderResponse.
    /// Implements the <see cref="IResponseObject" />
    /// </summary>
    /// <seealso cref="IResponseObject" />
    public class OrderResponse : IResponseObject
    {
        /// <summary>
        /// Indicates the response contains an order object. Will always contain order for this endpoint.
        /// </summary>
        /// <value>The resource.</value>
        public string Resource { get; set; }

        /// <summary>
        /// The order’s unique identifier, for example ord_vsKJpSsabw.
        /// </summary>
        /// <value>The identifier.</value>
        public string Id { get; set; }

        /// <summary>
        /// The profile the order was created on, for example pfl_v9hTwCvYqw.
        /// </summary>
        /// <value>The profile identifier.</value>
        public string ProfileId { get; set; }

        /// <summary>
        /// The payment method last used when paying for the order.
        /// </summary>
        /// <value>The method.</value>
        [JsonConverter(typeof(StringEnumConverter))]
        public PaymentMethods? Method { get; set; }

        /// <summary>
        /// The mode used to create this order.
        /// </summary>
        /// <value>The mode.</value>
        [JsonConverter(typeof(StringEnumConverter))]
        public PaymentMode Mode { get; set; }

        /// <summary>
        /// The total amount of the order, including VAT and discounts.
        /// </summary>
        /// <value>The amount.</value>
        public Amount Amount { get; set; }

        /// <summary>
        /// The amount captured, thus far. The captured amount will be settled to your account.
        /// </summary>
        /// <value>The amount captured.</value>
        public Amount AmountCaptured { get; set; }

        /// <summary>
        /// The total amount refunded, thus far.
        /// </summary>
        /// <value>The amount refunded.</value>
        public Amount AmountRefunded { get; set; }

        /// <summary>
        /// The status of the order.
        /// </summary>
        /// <value>The status.</value>
        public OrderStatus Status { get; set; }

        /// <summary>
        /// Whether or not the order can be (partially) canceled.
        /// </summary>
        /// <value><c>true</c> if this instance is cancelable; otherwise, <c>false</c>.</value>
        public bool IsCancelable { get; set; }

        /// <summary>
        /// The person and the address the order is billed to. See below.
        /// </summary>
        /// <value>The billing address.</value>
        public OrderAddressDetails BillingAddress { get; set; }

        /// <summary>
        /// The date of birth of your customer, if available.
        /// </summary>
        /// <value>The consumer date of birth.</value>
        public DateTime? ConsumerDateOfBirth { get; set; }

        /// <summary>
        /// Your order number that was used when creating the order.
        /// </summary>
        /// <value>The order number.</value>
        public string OrderNumber { get; set; }

        /// <summary>
        /// The person and the address the order is billed to. See below.
        /// </summary>
        /// <value>The shipping address.</value>
        public OrderAddressDetails ShippingAddress { get; set; }

        /// <summary>
        /// The locale used during checkout.
        /// </summary>
        /// <value>The locale.</value>
        public string Locale { get; set; }

        /// <summary>
        /// The optional metadata you provided upon subscription creation. Metadata can for example be used to link a plan to a
        /// subscription.
        /// </summary>
        /// <value>The metadata.</value>
        [JsonConverter(typeof(RawJsonConverter))]
        public string Metadata { get; set; }

        /// <summary>
        /// The URL your customer will be redirected to after completing or canceling the payment process.
        /// </summary>
        /// <value>The redirect URL.</value>
        public string RedirectUrl { get; set; }

        /// <summary>
        /// The URL ISynergy.Framework.Payment.Mollie will call as soon an important status change on the order takes place.
        /// </summary>
        /// <value>The webhook URL.</value>
        public string WebhookUrl { get; set; }

        /// <summary>
        /// The order’s date and time of creation, in ISO 8601 format.
        /// </summary>
        /// <value>The created at.</value>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// The date and time the order will expire, in ISO 8601 format. Note that you have until this date to fully ship the
        /// order.
        /// </summary>
        /// <value>The expires at.</value>
        public DateTime? ExpiresAt { get; set; }

        /// <summary>
        /// If the order is expired, the time of expiration will be present in ISO 8601 format.
        /// </summary>
        /// <value>The expired at.</value>
        public DateTime? ExpiredAt { get; set; }

        /// <summary>
        /// If the order has been paid, the time of payment will be present in ISO 8601 format.
        /// </summary>
        /// <value>The paid at.</value>
        public DateTime? PaidAt { get; set; }

        /// <summary>
        /// If the order has been authorized, the time of authorization will be present in ISO 8601 format.
        /// </summary>
        /// <value>The authorized at.</value>
        public DateTime? AuthorizedAt { get; set; }

        /// <summary>
        /// If the order has been canceled, the time of cancellation will be present in ISO 8601 format.
        /// </summary>
        /// <value>The canceled at.</value>
        public DateTime? CanceledAt { get; set; }

        /// <summary>
        /// If the order is completed, the time of completion will be present in ISO 8601 format.
        /// </summary>
        /// <value>The completed at.</value>
        public DateTime? CompletedAt { get; set; }

        /// <summary>
        /// Gets or sets the lines.
        /// </summary>
        /// <value>The lines.</value>
        public IEnumerable<OrderLineResponse> Lines { get; set; }

        /// <summary>
        /// An object with several URL objects relevant to the order. Every URL object will contain an href and a type field.
        /// </summary>
        /// <value>The links.</value>
        [JsonProperty("_links")]
        public OrderResponseLinks Links { get; set; }

        /// <summary>
        /// Gets the metadata.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonSerializerSettings">The json serializer settings.</param>
        /// <returns>T.</returns>
        public T GetMetadata<T>(JsonSerializerSettings jsonSerializerSettings = null)
        {
            return JsonSerializer.Deserialize<T>(Metadata, jsonSerializerSettings);
        }
    }
}
