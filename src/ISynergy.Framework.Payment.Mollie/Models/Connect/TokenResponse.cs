﻿using ISynergy.Framework.Payment.Mollie.Abstractions.Models;

namespace ISynergy.Framework.Payment.Mollie.Models.Connect
{
    /// <summary>
    /// Class TokenResponse.
    /// Implements the <see cref="IResponseObject" />
    /// </summary>
    /// <seealso cref="IResponseObject" />
    public class TokenResponse : IResponseObject
    {
        /// <summary>
        /// The access token, with which you will be able to access the ISynergy.Framework.Payment.Mollie API on the merchant's behalf.
        /// </summary>
        /// <value>The access token.</value>
        public string AccessToken { get; set; }

        /// <summary>
        /// The refresh token, with which you will be able to retrieve new access tokens on this endpoint. Please note that the
        /// refresh token does not expire.
        /// </summary>
        /// <value>The refresh token.</value>
        public string RefreshToken { get; set; }

        /// <summary>
        /// The number of seconds left before the access token expires. Be sure to renew your access token before this reaches
        /// zero.
        /// </summary>
        /// <value>The expires in.</value>
        public int ExpiresIn { get; set; }

        /// <summary>
        /// As per OAuth standards, the provided access token can only be used with bearer authentication.
        /// Possible values: bearer
        /// </summary>
        /// <value>The type of the token.</value>
        public string TokenType { get; set; }

        /// <summary>
        /// A space separated list of permissions. Please refer to OAuth: Permissions for the full permission list.
        /// </summary>
        /// <value>The scope.</value>
        public string Scope { get; set; }
    }
}
