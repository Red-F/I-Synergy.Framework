﻿using ISynergy.Framework.Payment.Mollie.Enumerations;

namespace ISynergy.Framework.Payment.Mollie.Models.Order
{
    /// <summary>
    /// Class OrderLineRequest.
    /// </summary>
    public class OrderLineRequest
    {
        /// <summary>
        /// The type of product bought, for example, a physical or a digital product. Must be one of the following values:
        /// physical (default), discount, digital, shipping_fee, store_credit, gift_card, surcharge
        /// </summary>
        /// <value>The type.</value>
        public OrderLineDetailsType? Type { get; set; }

        /// <summary>
        /// A description of the order line, for example LEGO 4440 Forest Police Station.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// The number of items in the order line.
        /// </summary>
        /// <value>The quantity.</value>
        public int Quantity { get; set; }

        /// <summary>
        /// The price of a single item in the order line.
        /// </summary>
        /// <value>The unit price.</value>
        public Amount UnitPrice { get; set; }

        /// <summary>
        /// Any discounts applied to the order line. For example, if you have a two-for-one sale, you should pass the
        /// amount discounted as a positive amount.
        /// </summary>
        /// <value>The discount amount.</value>
        public Amount DiscountAmount { get; set; }

        /// <summary>
        /// The total amount of the line, including VAT and discounts. Adding all totalAmount values together should
        /// result in the same amount as the amount top level property.
        /// </summary>
        /// <value>The total amount.</value>
        public Amount TotalAmount { get; set; }

        /// <summary>
        /// The VAT rate applied to the order line, for example "21.00" for 21%. The vatRate should be passed as a
        /// string and not as a float to ensure the correct number of decimals are passed.
        /// </summary>
        /// <value>The vat rate.</value>
        public string VatRate { get; set; }

        /// <summary>
        /// The amount of value-added tax on the line. The totalAmount field includes VAT, so the vatAmount can be
        /// calculated with the formula totalAmount × (vatRate / (100 + vatRate)). Any deviations from this will
        /// result in an error.
        /// </summary>
        /// <value>The vat amount.</value>
        public Amount VatAmount { get; set; }

        /// <summary>
        /// The SKU, EAN, ISBN or UPC of the product sold. The maximum character length is 64.
        /// </summary>
        /// <value>The sku.</value>
        public string Sku { get; set; }

        /// <summary>
        /// A link pointing to an image of the product sold.
        /// </summary>
        /// <value>The image URL.</value>
        public string ImageUrl { get; set; }

        /// <summary>
        /// A link pointing to the product page in your web shop of the product sold.
        /// </summary>
        /// <value>The product URL.</value>
        public string ProductUrl { get; set; }
    }
}
