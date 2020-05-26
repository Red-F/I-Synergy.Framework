﻿using System.Threading.Tasks;
using ISynergy.Framework.Core.Data;
using ISynergy.Framework.Mvvm.Abstractions.Services;

namespace ISynergy.Framework.Windows.Services
{
    public class BusyService : ObservableClass, IBusyService
    {
        protected readonly ILanguageService LanguageService;

        public BusyService(ILanguageService language)
        {
            LanguageService = language;
        }

        /// <summary>
        /// Gets or sets the IsBusy property value.
        /// </summary>
        public bool IsBusy
        {
            get { return GetValue<bool>(); }
            set
            {
                SetValue(value);
                IsEnabled = !value;
            }
        }

        /// <summary>
        /// Gets or sets the IsEnabled property value.
        /// </summary>
        public bool IsEnabled
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the BusyMessage property value.
        /// </summary>
        public string BusyMessage
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public Task StartBusyAsync(string message = null)
        {
            if (message != null)
            {
                BusyMessage = message;
            }
            else
            {
                BusyMessage = LanguageService.GetString("PleaseWait");
            }

            IsBusy = true;
            return Task.CompletedTask;
        }

        public Task StartBusyAsync()
        {
            return this.StartBusyAsync(LanguageService.GetString("PleaseWait"));
        }

        public Task EndBusyAsync()
        {
            IsBusy = false;
            return Task.CompletedTask;
        }
    }
}
