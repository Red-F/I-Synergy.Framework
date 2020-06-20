﻿using System.Collections.Generic;
using System.Linq;
using Xunit;
using ISynergy.Framework.Payment.Extensions;

namespace ISynergy.Framework.Payment.Tests.Extensions
{
    /// <summary>
    /// Class DictionaryExtensionsTests.
    /// </summary>
    public class DictionaryExtensionsTests
    {

        /// <summary>
        /// Defines the test method CanCreateUrlQueryFromDictionary.
        /// </summary>
        [Fact]
        public void CanCreateUrlQueryFromDictionary()
        {
            // Arrange
            var parameters = new Dictionary<string, string>()
            {
                {"include", "issuers"},
                {"testmode", "true"}
            };
            var expected = "?include=issuers&testmode=true";

            // Act
            var result = parameters.ToQueryString();

            // Assert
            Assert.Equal(expected, result);
        }

        /// <summary>
        /// Defines the test method CanCreateUrlQueryFromEmptyDictionary.
        /// </summary>
        [Fact]
        public void CanCreateUrlQueryFromEmptyDictionary()
        {
            // Arrange
            var parameters = new Dictionary<string, string>();
            var expected = string.Empty;

            // Act
            var result = parameters.ToQueryString();

            // Assert
            Assert.Equal(expected, result);
        }

        /// <summary>
        /// Defines the test method CanAddParameterToDictionaryIfNotEmptyDictionary.
        /// </summary>
        [Fact]
        public void CanAddParameterToDictionaryIfNotEmptyDictionary()
        {
            // Arrange
            var parameters = new Dictionary<string, string>();
            var parameterName = "include";
            var parameterValue = "issuers";

            // Act
            parameters.AddValueIfNotNullOrEmpty(parameterName, parameterValue);

            // Assert
            Assert.True(parameters.Any());
            Assert.Equal(parameterValue, parameters[parameterName]);
        }

        /// <summary>
        /// Defines the test method CannotAddParameterToDictionaryIfEmptyDictionary.
        /// </summary>
        [Fact]
        public void CannotAddParameterToDictionaryIfEmptyDictionary()
        {
            // Arrange
            var parameters = new Dictionary<string, string>();

            // Act
            parameters.AddValueIfNotNullOrEmpty("include", "");

            // Assert
            Assert.False(parameters.Any());
        }
    }
}
