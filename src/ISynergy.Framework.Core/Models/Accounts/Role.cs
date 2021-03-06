﻿using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using ISynergy.Framework.Core.Data;

namespace ISynergy.Framework.Core.Models.Accounts
{
    /// <summary>
    /// Class Role.
    /// Implements the <see cref="ModelBase" />
    /// </summary>
    /// <seealso cref="ModelBase" />
    public class Role : ModelBase
    {
        /// <summary>
        /// Gets or sets the Id property value.
        /// </summary>
        /// <value>The identifier.</value>
        [Required]
        public Guid Id
        {
            get { return GetValue<Guid>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Name property value.
        /// </summary>
        /// <value>The name.</value>
        [Required]
        public string Name
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        [JsonIgnore]
        public string Description { get; set; } = string.Empty;
    }
}
