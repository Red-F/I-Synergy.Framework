﻿using System;

namespace ISynergy.Framework.Core.Data
{
    /// <summary>
    /// Interface IEntityBase
    /// Implements the <see cref="IClassBase" />
    /// </summary>
    /// <seealso cref="IClassBase" />
    public interface IEntityBase : IClassBase
    {
        /// <summary>
        /// Gets or sets the memo.
        /// </summary>
        /// <value>The memo.</value>
        string Memo { get; set; }
        /// <summary>
        /// Gets or sets the created date.
        /// </summary>
        /// <value>The created date.</value>
        DateTimeOffset CreatedDate { get; set; }
        /// <summary>
        /// Gets or sets the changed date.
        /// </summary>
        /// <value>The changed date.</value>
        DateTimeOffset? ChangedDate { get; set; }
        /// <summary>
        /// Gets or sets the created by.
        /// </summary>
        /// <value>The created by.</value>
        string CreatedBy { get; set; }
        /// <summary>
        /// Gets or sets the changed by.
        /// </summary>
        /// <value>The changed by.</value>
        string ChangedBy { get; set; }
    }
}
