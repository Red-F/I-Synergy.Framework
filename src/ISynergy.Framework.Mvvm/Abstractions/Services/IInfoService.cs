﻿namespace ISynergy.Framework.Mvvm.Abstractions.Services
{
    /// <summary>
    /// Interface IInfoService
    /// </summary>
    public interface IInfoService
    {
        /// <summary>
        /// Gets the application path.
        /// </summary>
        /// <value>The application path.</value>
        string ApplicationPath { get; }
        /// <summary>
        /// Gets the name of the company.
        /// </summary>
        /// <value>The name of the company.</value>
        string CompanyName { get; }
        /// <summary>
        /// Gets the product version.
        /// </summary>
        /// <value>The product version.</value>
        string ProductVersion { get; }
        /// <summary>
        /// Gets the name of the product.
        /// </summary>
        /// <value>The name of the product.</value>
        string ProductName { get; }
    }
}
