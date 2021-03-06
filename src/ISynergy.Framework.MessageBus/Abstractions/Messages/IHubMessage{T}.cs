﻿namespace ISynergy.Framework.MessageBus.Abstractions
{
    /// <summary>
    /// Interface IHubMessage
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    interface IHubMessage<TEntity> : IBaseMessage
    {
        /// <summary>
        /// Gets the channel.
        /// </summary>
        /// <value>The channel.</value>
        public string Channel { get; }
        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <value>The data.</value>
        public TEntity Data { get; }

        /// <summary>
        /// Gets the event.
        /// </summary>
        /// <value>The event.</value>
        string Event { get; }
    }
}
