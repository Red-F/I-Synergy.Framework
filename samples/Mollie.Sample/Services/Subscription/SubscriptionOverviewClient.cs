﻿using System.Threading.Tasks;
using AutoMapper;
using ISynergy.Framework.Payment.Mollie.Abstractions.Clients;
using ISynergy.Framework.Payment.Mollie.Models.Subscription;
using Mollie.Sample.Models;

namespace Mollie.Sample.Services.Subscription
{
    /// <summary>
    /// Class SubscriptionOverviewClient.
    /// Implements the <see cref="OverviewClientBase{SubscriptionResponse}" />
    /// Implements the <see cref="ISubscriptionOverviewClient" />
    /// </summary>
    /// <seealso cref="OverviewClientBase{SubscriptionResponse}" />
    /// <seealso cref="ISubscriptionOverviewClient" />
    /// <autogeneratedoc />
    public class SubscriptionOverviewClient : OverviewClientBase<SubscriptionResponse>, ISubscriptionOverviewClient {
        /// <summary>
        /// The subscription client
        /// </summary>
        /// <autogeneratedoc />
        private readonly ISubscriptionClient _subscriptionClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionOverviewClient"/> class.
        /// </summary>
        /// <param name="mapper">The mapper.</param>
        /// <param name="subscriptionClient">The subscription client.</param>
        /// <autogeneratedoc />
        public SubscriptionOverviewClient(IMapper mapper, ISubscriptionClient subscriptionClient) : base(mapper) {
            _subscriptionClient = subscriptionClient;
        }

        /// <summary>
        /// Gets the list.
        /// </summary>
        /// <param name="customerId">The customer identifier.</param>
        /// <returns>Task&lt;OverviewModel&lt;SubscriptionResponse&gt;&gt;.</returns>
        /// <autogeneratedoc />
        public async Task<OverviewModel<SubscriptionResponse>> GetList(string customerId) {
            return Map(await _subscriptionClient.GetSubscriptionListAsync(customerId));
        }

        /// <summary>
        /// Gets the list by URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>OverviewModel&lt;SubscriptionResponse&gt;.</returns>
        /// <autogeneratedoc />
        public async Task<OverviewModel<SubscriptionResponse>> GetListByUrl(string url) {
            return Map(await _subscriptionClient.GetSubscriptionListAsync(CreateUrlObject(url)));
        }
    }
}
