﻿using Microsoft.Extensions.Configuration;

namespace Mollie.Sample.Framework.Extensions
{
    /// <summary>
    /// Class ConfigurationBinderExtensions.
    /// </summary>
    /// <autogeneratedoc />
    public static class ConfigurationBinderExtensions
    {
        /// <summary>
        /// Binds the with reload.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="instance">The instance.</param>
        /// <autogeneratedoc />
        public static void BindWithReload(this IConfiguration configuration, object instance)
        {
            configuration.Bind(instance);
            configuration.GetReloadToken().RegisterChangeCallback((_) => configuration.Bind(instance), null);
        }
    }
}
