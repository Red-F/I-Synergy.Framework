﻿using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xunit;

namespace ISynergy.Framework.Core.Extensions.Tests
{
    public class CollectionExtensionsTests
    {
        [Fact]
        public void NullObservableCollectionNonFailableTest()
        {
            ObservableCollection<object> list = null;
            var result = false;

            foreach (var item in list.EnsureNotNull()) { }

            result = true;

            Assert.True(result);
        }

        [Fact]
        public void NullObservableCollectionFailableTest()
        {
            Assert.ThrowsAsync<NullReferenceException>(() =>
            {
                ObservableCollection<object> list = null;

                foreach (var item in list) { }

                return Task.CompletedTask;
            });
        }
    }
}
