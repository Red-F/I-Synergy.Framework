﻿using System;
using System.Threading.Tasks;
using ISynergy.Framework.Mvvm.Abstractions.Services;

namespace ISynergy.Framework.UI.Sample.Services
{
    /// <summary>
    /// Class CameraService.
    /// </summary>
    public class CameraService : ICameraService
    {
        public Task<FileResult> TakePictureAsync(ulong maxfilesize)
        {
            throw new NotImplementedException();
        }
    }
}
