﻿using Microsoft.Extensions.Logging;

namespace ISynergy.Framework.Payment.Clients
{
    /// <summary>
    /// Class ClientBase.
    /// </summary>
    /// <autogeneratedoc />
    public abstract class ClientBase
    {
        /// <summary>
        /// The logger
        /// </summary>
        /// <autogeneratedoc />
        protected readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientBase"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <autogeneratedoc />
        protected ClientBase(ILogger<ClientBase> logger)
        {
            _logger = logger;
        }
    }
}
