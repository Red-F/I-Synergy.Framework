﻿using System.Collections.Generic;

namespace ISynergy.Framework.Mathematics
{
    /// <summary>
    /// Class Hash.
    /// </summary>
    public class Hash
    {
        // hash table in format number of nonzero cell = cell value
        /// <summary>
        /// The hash
        /// </summary>
        private readonly Dictionary<int, double> hash = new Dictionary<int, double>();

        /// <summary>
        /// Method for set value in hash
        /// </summary>
        /// <param name="num">Value</param>
        /// <param name="value">Key</param>
        public void SetValue(int num, double value)
        {
            // if the value is 0, then delete this cell
            if (value == 0)
            {
                if (hash.ContainsKey(num))
                    hash.Remove(num);
                return;
            }
            // if the value is not 0, we overwrite or add a cell
            hash[num] = value;
        }

        /// <summary>
        /// Method for add for ext item some value
        /// </summary>
        /// <param name="num">Value</param>
        /// <param name="value">Key</param>
        public void AddValue(int num, double value)
        {
            if (hash.ContainsKey(num))
            {
                hash[num] = (double)hash[num] + value;
            }
            else
            {
                hash[num] = value;
            }
        }

        /// <summary>
        /// Method for get value from hash
        /// </summary>
        /// <param name="num">Key</param>
        /// <returns>return value if hash contain num or return 0</returns>
        public double GetValue(int num)
        {
            if (hash.ContainsKey(num))
                return (double)hash[num];

            return 0;
        }
    }
}
