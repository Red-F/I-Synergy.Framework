﻿using System.Collections.Generic;
using ISynergy.Framework.Core.Validation;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace ISynergy.Framework.UI.Dialogs
{
    /// <summary>
    /// Class wrapping <see cref="FileOpenPicker" />.
    /// </summary>
    internal sealed class FileOpenPickerWrapper
    {
        /// <summary>
        /// The picker
        /// </summary>
        private readonly FileOpenPicker picker;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileOpenPickerWrapper" /> class.
        /// </summary>
        /// <param name="settings">The settings for the file open picker.</param>
        internal FileOpenPickerWrapper(FileOpenPickerSettings settings)
        {
            Argument.IsNotNull(nameof(settings), settings);

            picker = new FileOpenPicker
            {
                CommitButtonText = settings.CommitButtonText,
                SettingsIdentifier = settings.SettingsIdentifier,
                SuggestedStartLocation = settings.SuggestedStartLocation,
                ViewMode = settings.ViewMode
            };

            foreach (var fileTypeFilter in settings.FileTypeFilter)
            {
                picker.FileTypeFilter.Add(fileTypeFilter);
            }
        }

        /// <summary>
        /// Shows the file picker so that the user can pick one file.
        /// </summary>
        /// <returns>When the call to this method completes successfully, it returns a
        /// <see cref="StorageFile" /> object that represents the file that the user picked.</returns>
        internal IAsyncOperation<StorageFile> PickSingleFileAsync() => picker.PickSingleFileAsync();

        /// <summary>
        /// Shows the file picker so that the user can pick multiple files.
        /// </summary>
        /// <returns>When the call to this method completes successfully, it returns a
        /// <see cref="IReadOnlyList{StorageFile}" /> object that contains all the files that were
        /// picked by the user. Picked files in this array are represented by
        /// <see cref="StorageFile" /> objects.</returns>
        internal IAsyncOperation<IReadOnlyList<StorageFile>> PickMultipleFilesAsync() => picker.PickMultipleFilesAsync();
    }
}
