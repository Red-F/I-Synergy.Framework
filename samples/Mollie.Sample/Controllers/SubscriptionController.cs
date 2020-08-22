﻿using System.Threading.Tasks;
using ISynergy.Framework.Payment.Mollie.Models;
using Microsoft.AspNetCore.Mvc;
using Mollie.Sample.Models;
using Mollie.Sample.Services.Subscription;

namespace Mollie.Sample.Controllers
{
    /// <summary>
    /// Class SubscriptionController.
    /// Implements the <see cref="Controller" />
    /// </summary>
    /// <seealso cref="Controller" />
    /// <autogeneratedoc />
    public class SubscriptionController : Controller {
        /// <summary>
        /// The subscription overview client
        /// </summary>
        /// <autogeneratedoc />
        private readonly ISubscriptionOverviewClient _subscriptionOverviewClient;
        /// <summary>
        /// The subscription storage client
        /// </summary>
        /// <autogeneratedoc />
        private readonly ISubscriptionStorageClient _subscriptionStorageClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionController"/> class.
        /// </summary>
        /// <param name="subscriptionOverviewClient">The subscription overview client.</param>
        /// <param name="subscriptionStorageClient">The subscription storage client.</param>
        /// <autogeneratedoc />
        public SubscriptionController(ISubscriptionOverviewClient subscriptionOverviewClient, ISubscriptionStorageClient subscriptionStorageClient) {
            _subscriptionOverviewClient = subscriptionOverviewClient;
            _subscriptionStorageClient = subscriptionStorageClient;
        }

        /// <summary>
        /// Indexes the specified customer identifier.
        /// </summary>
        /// <param name="customerId">The customer identifier.</param>
        /// <returns>ViewResult.</returns>
        /// <autogeneratedoc />
        [HttpGet]
        public async Task<ViewResult> Index(string customerId) {
            ViewBag.CustomerId = customerId;
            var model = await _subscriptionOverviewClient.GetList(customerId);
            return View(model);
        }

        /// <summary>
        /// APIs the URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>ViewResult.</returns>
        /// <autogeneratedoc />
        [HttpGet]
        public async Task<ViewResult> ApiUrl([FromQuery]string url) {
            var model = await _subscriptionOverviewClient.GetListByUrl(url);
            return View(nameof(this.Index), model);
        }

        /// <summary>
        /// Creates the specified customer identifier.
        /// </summary>
        /// <param name="customerId">The customer identifier.</param>
        /// <returns>ViewResult.</returns>
        /// <autogeneratedoc />
        [HttpGet]
        public ViewResult Create(string customerId) {
            var model = new CreateSubscriptionModel() {
                CustomerId = customerId,
                Amount = 10,
                Currency = Currency.EUR
            };

            return View(model);
        }

        /// <summary>
        /// Creates the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>IActionResult.</returns>
        /// <autogeneratedoc />
        [HttpPost]
        public async Task<IActionResult> Create(CreateSubscriptionModel model) {
            if (!ModelState.IsValid) {
                return View();
            }

            await _subscriptionStorageClient.Create(model);
            return RedirectToAction(nameof(this.Index), new { customerId = model.CustomerId });
        }
    }
}
