﻿using System.Text.Json.Serialization;

namespace ISynergy.Framework.Core.Models
{
    /// <summary>
    /// Class Grant.
    /// </summary>
    public class Grant
    {
        /// <summary>
        /// Gets or sets the type of the grant.
        /// </summary>
        /// <value>The type of the grant.</value>
        [JsonPropertyName("grant_type")] public string grant_type { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        /// <value>The username.</value>
        [JsonPropertyName("username")] public string username { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>The password.</value>
        [JsonPropertyName("password")] public string password { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the refresh token.
        /// </summary>
        /// <value>The refresh token.</value>
        [JsonPropertyName("refresh_token")] public string refresh_token { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the client identifier.
        /// </summary>
        /// <value>The client identifier.</value>
        [JsonPropertyName("client_id")] public string client_id { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the client secret.
        /// </summary>
        /// <value>The client secret.</value>
        [JsonPropertyName("client_secret")] public string client_secret { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the scope.
        /// </summary>
        /// <value>The scope.</value>
        [JsonPropertyName("scope")] public string scope { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>The code.</value>
        [JsonPropertyName("code")] public string code { get; set; } = string.Empty;
    }

    /// <summary>
    /// Class GrantTypes.
    /// </summary>
    public static class GrantTypes
    {
        /// <summary>
        /// The password
        /// </summary>
        public const string Password = "password";
        /// <summary>
        /// The refresh token
        /// </summary>
        public const string RefreshToken = "refresh_token";
        /// <summary>
        /// The client credentials
        /// </summary>
        public const string ClientCredentials = "client_credentials";
        /// <summary>
        /// The api key.
        /// </summary>
        public const string ApiKey = "api_key";
    }

    /// <summary>
    /// Class GrantScopes.
    /// </summary>
    public static class GrantScopes
    {
        /// <summary>
        /// The password
        /// </summary>
        public const string Password = "openid offline_access";
        /// <summary>
        /// The client credentials
        /// </summary>
        public const string ClientCredentials = "openid";
    }
}
