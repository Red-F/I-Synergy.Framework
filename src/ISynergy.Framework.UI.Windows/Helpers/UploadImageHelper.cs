﻿using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace ISynergy.Framework.UI.Helpers
{
    /// <summary>
    /// Class UploadImageHelper.
    /// </summary>
    public static class UploadImageHelper
    {
        /// <summary>
        /// upload image as an asynchronous operation.
        /// </summary>
        /// <returns>System.String.</returns>
        public static async Task<string> UploadImageAsync()
        {
            var picker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.PicturesLibrary
            };

            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".bmp");
            picker.FileTypeFilter.Add(".gif");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");

            var file = await picker.PickSingleFileAsync();
            return await SaveImageAsync(file);
        }

        /// <summary>
        /// save image as an asynchronous operation.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>System.String.</returns>
        private static async Task<string> SaveImageAsync(StorageFile file)
        {
            if (file is null)
            {
                return string.Empty;
            }

            var appInstalledFolder = Package.Current.InstalledLocation;
            var assets = await appInstalledFolder.GetFolderAsync("Assets");

            var targetFile = await assets.CreateFileAsync(file.Name, CreationCollisionOption.GenerateUniqueName);
            await file.CopyAndReplaceAsync(targetFile);
            return targetFile.Path;
        }
    }
}
