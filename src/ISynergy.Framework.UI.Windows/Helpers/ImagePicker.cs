﻿using System;
using System.IO;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace ISynergy.Framework.UI.Helpers
{
    /// <summary>
    /// Class ImagePicker.
    /// </summary>
    public class ImagePicker
    {
        /// <summary>
        /// Gets the file path.
        /// </summary>
        /// <value>The file path.</value>
        public string FilePath { get; private set; }
        /// <summary>
        /// Gets the type of the content.
        /// </summary>
        /// <value>The type of the content.</value>
        public string ContentType { get; private set; }

        /// <summary>
        /// get image as an asynchronous operation.
        /// </summary>
        /// <returns>System.Byte[].</returns>
        public async Task<byte[]> GetImageAsync()
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
            return await LoadImageAsync(file);
        }

        /// <summary>
        /// load image as an asynchronous operation.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>System.Byte[].</returns>
        private async Task<byte[]> LoadImageAsync(StorageFile file)
        {
            if (file is null)
            {
                return null;
            }

            ContentType = file.ContentType;

            var appInstalledFolder = Package.Current.InstalledLocation;
            var assets = await appInstalledFolder.GetFolderAsync("Assets");

            var targetFile = await assets.CreateFileAsync(file.Name, CreationCollisionOption.GenerateUniqueName);
            await file.CopyAndReplaceAsync(targetFile);
            FilePath = targetFile.Path;

            using var randomStream = await file.OpenReadAsync();
            using var stream = randomStream.AsStream();
            var buffer = new byte[randomStream.Size];
            await stream.ReadAsync(buffer, 0, buffer.Length);
            return buffer;
        }
    }
}
