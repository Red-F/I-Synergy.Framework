﻿using System.Threading.Tasks;
using ISynergy.Framework.Payment.Mollie.Abstractions.Clients;
using ISynergy.Framework.Payment.Mollie.Base;
using ISynergy.Framework.Payment.Mollie.Models.List;
using ISynergy.Framework.Payment.Mollie.Models.Profile.Request;
using ISynergy.Framework.Payment.Mollie.Models.Profile.Response;
using ISynergy.Framework.Payment.Mollie.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ISynergy.Framework.Payment.Mollie.Clients
{
    /// <summary>
    /// Class ProfileClient.
    /// Implements the <see cref="MollieClientBase" />
    /// Implements the <see cref="IProfileClient" />
    /// </summary>
    /// <seealso cref="MollieClientBase" />
    /// <seealso cref="IProfileClient" />
    public class ProfileClient : MollieClientBase, IProfileClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProfileClient" /> class.
        /// </summary>
        /// <param name="clientService">The client service.</param>
        /// <param name="options">The options.</param>
        /// <param name="logger">The logger.</param>
        public ProfileClient(IMollieClientService clientService, IOptions<MollieApiOptions> options, ILogger<ProfileClient> logger) : base(clientService, options, logger)
        {
        }

        /// <summary>
        /// Creates the profile asynchronous.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>Task&lt;ProfileResponse&gt;.</returns>
        public Task<ProfileResponse> CreateProfileAsync(ProfileRequest request) =>
            _clientService.PostAsync<ProfileResponse>("profiles", request);

        /// <summary>
        /// Gets the profile asynchronous.
        /// </summary>
        /// <param name="profileId">The profile identifier.</param>
        /// <returns>Task&lt;ProfileResponse&gt;.</returns>
        public Task<ProfileResponse> GetProfileAsync(string profileId) =>
            _clientService.GetAsync<ProfileResponse>($"profiles/{profileId}");

        /// <summary>
        /// Gets the profile list asynchronous.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="limit">The limit.</param>
        /// <returns>Task&lt;ListResponse&lt;ProfileResponse&gt;&gt;.</returns>
        public Task<ListResponse<ProfileResponse>> GetProfileListAsync(string from = null, int? limit = null) =>
            _clientService.GetListAsync<ListResponse<ProfileResponse>>("profiles", from, limit);

        /// <summary>
        /// Updates the profile asynchronous.
        /// </summary>
        /// <param name="profileId">The profile identifier.</param>
        /// <param name="request">The request.</param>
        /// <returns>Task&lt;ProfileResponse&gt;.</returns>
        public Task<ProfileResponse> UpdateProfileAsync(string profileId, ProfileRequest request) =>
            _clientService.PostAsync<ProfileResponse>($"profiles/{profileId}", request);

        /// <summary>
        /// Deletes the profile asynchronous.
        /// </summary>
        /// <param name="profileId">The profile identifier.</param>
        /// <returns>Task.</returns>
        public Task DeleteProfileAsync(string profileId) =>
            _clientService.DeleteAsync($"profiles/{profileId}");
    }
}
