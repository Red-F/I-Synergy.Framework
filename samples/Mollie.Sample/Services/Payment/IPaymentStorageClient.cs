﻿using System.Threading.Tasks;
using Mollie.Sample.Models;

namespace Mollie.Sample.Services.Payment {
    /// <summary>
    /// Interface IPaymentStorageClient
    /// </summary>
    /// <autogeneratedoc />
    public interface IPaymentStorageClient {
        /// <summary>
        /// Creates the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>Task.</returns>
        /// <autogeneratedoc />
        Task Create(CreatePaymentModel model);
    }
}
