﻿using System.Threading.Tasks;
using ISynergy.Framework.UI.Sample.Abstractions.Services;

namespace ISynergy.Framework.UI.Sample.Services
{
    /// <summary>
    /// Class ClientMonitorService.
    /// </summary>
    public class ClientMonitorService : IClientMonitorService
    {
        public Task ConnectAsync(string token)
        {
            throw new System.NotImplementedException();
        }

        public Task DisconnectAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}
