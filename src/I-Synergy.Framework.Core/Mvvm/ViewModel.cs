﻿using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using ISynergy.Models.Base;
using ISynergy.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using ISynergy.Events;
using System.Collections;
using ISynergy.Helpers;
using System.Linq;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using GalaSoft.MvvmLight;

namespace ISynergy.Mvvm
{
    public abstract class ViewModel : ViewModelBase, IViewModel
    {
        public delegate Task Submit_Action(object e);

        public IContext Context { get; }
        public IBaseService BaseService { get; }
        public IValidationService ValidationService { get; }

        public RelayCommand Close_Command { get; protected set; }

        /// <summary>
        /// Gets or sets the Title property value.
        /// </summary>
        public virtual string Title
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the NumberInfo property value.
        /// </summary>
        public NumberFormatInfo NumberFormat
        {
            get { return GetValue<NumberFormatInfo>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Culture property value.
        /// </summary>
        public CultureInfo Culture
        {
            get { return GetValue<CultureInfo>(); }
            set { SetValue(value); }
        }
        
        /// <summary>
        /// Gets or sets the IsInitialized property value.
        /// </summary>
        public bool IsInitialized
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Errors property value.
        /// </summary>
        public List<string> Errors
        {
            get { return GetValue<List<string>>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Mode_IsAdvanced property value.
        /// </summary>
        public bool Mode_IsAdvanced
        {
            get { return BaseService.BaseSettingsService.Application_Advanced; }
            set
            {
                BaseService.BaseSettingsService.Application_Advanced = value;

                if (value)
                {
                    Mode_ToolTip = BaseService.LanguageService.GetString("Generic_Advanced");
                }
                else
                {
                    Mode_ToolTip = BaseService.LanguageService.GetString("Generic_Basic");
                }
            }
        }

        /// <summary>
        /// Gets or sets the Mode_ToolTip property value.
        /// </summary>
        public string Mode_ToolTip
        {
            get
            {
                string result = GetValue<string>();

                if (string.IsNullOrWhiteSpace(result))
                {
                    if (Mode_IsAdvanced)
                    {
                        result = BaseService.LanguageService.GetString("Generic_Advanced");
                    }
                    else
                    {
                        result = BaseService.LanguageService.GetString("Generic_Basic");
                    }
                }

                return result;
            }
            set { SetValue(value); }
        }

        private WeakEventListener<IViewModel, object, PropertyChangedEventArgs> WeakViewModelPropertyChangedEvent = null;
        private WeakEventListener<IViewModel, object, DataErrorsChangedEventArgs> WeakValidationErrorsChangedEvent = null;

        public ViewModel(
            IContext context, 
            IBaseService baseService)
            : base()
        {
            Context = context;
            BaseService = baseService;
            ValidationService = baseService.ValidationService;

            WeakViewModelPropertyChangedEvent = new WeakEventListener<IViewModel, object, PropertyChangedEventArgs>(this)
            {
                OnEventAction = (instance, source, eventargs) => instance.OnPropertyChanged(source, eventargs),
                OnDetachAction = (listener) => this.PropertyChanged -= listener.OnEvent
            };

            this.PropertyChanged += WeakViewModelPropertyChangedEvent.OnEvent;

            WeakValidationErrorsChangedEvent = new WeakEventListener<IViewModel, object, DataErrorsChangedEventArgs>(this)
            {
                OnEventAction = (instance, source, eventargs) => instance.OnValidationErrorsChanged(source, eventargs),
                OnDetachAction = (listener) => ValidationService.ErrorsChanged -= listener.OnEvent
            };

            ValidationService.ErrorsChanged += WeakValidationErrorsChangedEvent.OnEvent;

            Messenger.Default.Register<ExceptionHandledMessage>(this, i => BaseService.BusyService.EndBusyAsync());

            Culture = Thread.CurrentThread.CurrentCulture;
            Culture.NumberFormat.CurrencySymbol = $"{Context.CurrencySymbol} ";
            Culture.NumberFormat.CurrencyNegativePattern = 1;

            NumberFormat = Culture.NumberFormat;
            
            IsInitialized = false;

            Close_Command = new RelayCommand(() =>
            {
                Messenger.Default.Send(new OnCancelMessage(this));
            });
        }

        public void OnValidationErrorsChanged(object sender, DataErrorsChangedEventArgs e)
        {
            Errors = ValidationService.GetErrorList();
        }

        public virtual async Task InitializeAsync()
        {
            await BaseService.TelemetryService.TrackPageViewAsync(GetType().Name.Replace("ViewModel", ""));
        }

        protected string GetEnumDescription(Enum value)
        {
            Argument.IsNotNull(nameof(value), value);

            string description = value.ToString();
            FieldInfo fieldInfo = value.GetType().GetField(description);
            DisplayAttribute[] attributes = (DisplayAttribute[])fieldInfo.GetCustomAttributes(typeof(DisplayAttribute), false);

            if (attributes != null && attributes.Length > 0)
            {
                description = BaseService.LanguageService.GetString(attributes[0].Description);
            }

            return description;
        }

        public virtual void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
        }

        public bool CanClose { get; set; }
        public bool IsCancelled { get; protected set; }

        protected virtual Task CancelViewModelAsync()
        {
            IsCancelled = true;
            return Task.CompletedTask;
        }

        public virtual Task OnDeactivateAsync()
        {
            Cleanup();
            return Task.CompletedTask;
        }

        public virtual Task OnActivateAsync(object parameter, bool isBack) => InitializeAsync();

        private readonly Dictionary<string, object> _propertyBackingDictionary = new Dictionary<string, object>();

        protected T GetValue<T>([CallerMemberName] string propertyName = null)
        {
            Argument.IsNotNull(propertyName, propertyName);

            if (_propertyBackingDictionary.TryGetValue(propertyName, out object value)) return (T)value;

            return default;
        }

        protected bool SetValue<T>(T newValue, [CallerMemberName] string propertyName = null)
        {
            Argument.IsNotNull(propertyName, propertyName);

            if (EqualityComparer<T>.Default.Equals(newValue, GetValue<T>(propertyName))) return false;

            _propertyBackingDictionary[propertyName] = newValue;

            RaisePropertyChanged(propertyName);

            if (!string.IsNullOrEmpty(propertyName))
                ValidationService.ValidateProperty(this.GetType(), propertyName);

            return true;
        }

        public virtual async Task<bool> ValidateInputAsync()
        {
            ValidationService.ValidateProperties();
            Errors = ValidationService.GetErrorList();

            if (ValidationService.HasErrors)
            {
                await BaseService.DialogService.ShowErrorAsync(
                    BaseService.LanguageService.GetString("Warning_Validation_Failed"));
                return false;
            }

            return true;
        }
    }
}