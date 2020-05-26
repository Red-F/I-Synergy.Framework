﻿using System.Collections.Generic;

namespace ISynergy.Framework.AspNetCore.WebDav.Server.Model.Headers
{
    /// <summary>
    /// Comparer for the <see cref="DepthHeader"/>
    /// </summary>
    public class DepthHeaderComparer : IComparer<DepthHeader>, IEqualityComparer<DepthHeader>
    {
        /// <summary>
        /// Gets the default depth header comparer
        /// </summary>
        public static DepthHeaderComparer Default { get; } = new DepthHeaderComparer();

        /// <inheritdoc />
        public int Compare(DepthHeader x, DepthHeader y)
        {
            return x.OrderValue.CompareTo(y.OrderValue);
        }

        /// <inheritdoc />
        public bool Equals(DepthHeader x, DepthHeader y)
        {
            return Compare(x, y) == 0;
        }

        /// <inheritdoc />
        public int GetHashCode(DepthHeader obj)
        {
            return obj.OrderValue.GetHashCode();
        }
    }
}
