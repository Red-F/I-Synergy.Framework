﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Mollie.Sample.Services.Mandate;

namespace Mollie.Sample.Controllers
{
    /// <summary>
    /// Class MandateController.
    /// Implements the <see cref="Controller" />
    /// </summary>
    /// <seealso cref="Controller" />
    /// <autogeneratedoc />
    public class MandateController : Controller {
        /// <summary>
        /// The mandate overview client
        /// </summary>
        /// <autogeneratedoc />
        private readonly IMandateOverviewClient _mandateOverviewClient;
        /// <summary>
        /// The mandate storage client
        /// </summary>
        /// <autogeneratedoc />
        private readonly IMandateStorageClient _mandateStorageClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="MandateController"/> class.
        /// </summary>
        /// <param name="mandateOverviewClient">The mandate overview client.</param>
        /// <param name="mandateStorageClient">The mandate storage client.</param>
        /// <autogeneratedoc />
        public MandateController(IMandateOverviewClient mandateOverviewClient, IMandateStorageClient mandateStorageClient) {
            _mandateOverviewClient = mandateOverviewClient;
            _mandateStorageClient = mandateStorageClient;
        }

        /// <summary>
        /// Indexes the specified customer identifier.
        /// </summary>
        /// <param name="customerId">The customer identifier.</param>
        /// <returns>ViewResult.</returns>
        /// <autogeneratedoc />
        public async Task<ViewResult> Index(string customerId) {
            ViewBag.CustomerId = customerId;
            var model = await _mandateOverviewClient.GetList(customerId);
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
            var model = await _mandateOverviewClient.GetListByUrl(url);
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
            ViewBag.CustomerId = customerId;
            return View();
        }

        /// <summary>
        /// Creates the post.
        /// </summary>
        /// <param name="customerId">The customer identifier.</param>
        /// <returns>IActionResult.</returns>
        /// <autogeneratedoc />
        [HttpPost]
        public async Task<IActionResult> CreatePost(string customerId) {
            await _mandateStorageClient.Create(customerId);
            return RedirectToAction("Index", new { customerId });
        }
    }
}
