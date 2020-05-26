﻿using Windows.ApplicationModel.Resources;

namespace ISynergy.Framework.Windows.Extensions
{
    public static class ResourceExtensions
    {
        private static readonly ResourceLoader ResLoader = new ResourceLoader();

        public static string GetLocalized(this string resourceKey)
        {
            return ResLoader.GetString(resourceKey);
        }
    }
}
