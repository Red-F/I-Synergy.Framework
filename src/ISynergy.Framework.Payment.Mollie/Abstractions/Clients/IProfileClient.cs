﻿using System.Threading.Tasks;
using ISynergy.Framework.Payment.Mollie.Models.List;

using ISynergy.Framework.Payment.Mollie.Models.Profile.Request;
using ISynergy.Framework.Payment.Mollie.Models.Profile.Response;

namespace ISynergy.Framework.Payment.Mollie.Abstractions.Clients
{
    /// <summary>
    /// Interface IProfileClient
    /// </summary>
    public interface IProfileClient
    {
        /// <summary>
        /// Creates the profile asynchronous.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>Task&lt;ProfileResponse&gt;.</returns>
        Task<ProfileResponse> CreateProfileAsync(ProfileRequest request);
        /// <summary>
        /// Gets the profile asynchronous.
        /// </summary>
        /// <param name="profileId">The profile identifier.</param>
        /// <returns>Task&lt;ProfileResponse&gt;.</returns>
        Task<ProfileResponse> GetProfileAsync(string profileId);
        /// <summary>
        /// Gets the profile list asynchronous.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="limit">The limit.</param>
        /// <returns>Task&lt;ListResponse&lt;ProfileResponse&gt;&gt;.</returns>
        Task<ListResponse<ProfileResponse>> GetProfileListAsync(string from = null, int? limit = null);
        /// <summary>
        /// Updates the profile asynchronous.
        /// </summary>
        /// <param name="profileId">The profile identifier.</param>
        /// <param name="request">The request.</param>
        /// <returns>Task&lt;ProfileResponse&gt;.</returns>
        Task<ProfileResponse> UpdateProfileAsync(string profileId, ProfileRequest request);
        /// <summary>
        /// Deletes the profile asynchronous.
        /// </summary>
        /// <param name="profileId">The profile identifier.</param>
        /// <returns>Task.</returns>
        Task DeleteProfileAsync(string profileId);
    }
}
