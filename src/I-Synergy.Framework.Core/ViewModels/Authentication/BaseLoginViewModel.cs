﻿using ISynergy.Handlers;
using ISynergy.Models.Accounts;
using ISynergy.Services;
using ISynergy.ViewModels.Base;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Linq;
using GalaSoft.MvvmLight.Command;
using System;
using ISynergy.Events;

namespace ISynergy.ViewModels.Authentication
{
    public abstract class BaseLoginViewModel : ViewModelNavigation<object>, ILoginViewModel
    {
        public RelayCommand ShowLogin_Command { get; set; }
        public RelayCommand Register_Command { get; set; }
        public RelayCommand ForgotPassword_Command { get; set; }

        public BaseLoginViewModel(
            IContext context,
            IBaseService baseService)
            : base(context, baseService)
        {
            Usernames = new List<string>();
            TimeZones = TimeZoneInfo.GetSystemTimeZones().ToList();
            Registration_TimeZone = "W. Europe Standard Time";

            LoginVisible = true;

            ShowLogin_Command = new RelayCommand(SetLoginVisibility);

            Register_Command = new RelayCommand(async () => await RegisterAsync());
            ForgotPassword_Command = new RelayCommand(async () => await ForgotPasswordAsync());

            if(BaseService.ApplicationSettings.Application_Users != null)
            {
                Usernames = JsonConvert.DeserializeObject<List<string>>(BaseService.ApplicationSettings.Application_Users);
            }
            
            Username = BaseService.ApplicationSettings.Application_User;
            AutoLogin = BaseService.ApplicationSettings.User_AutoLogin;
        }

        public Task CheckAutoLogin()
        {
            if (BaseService.ApplicationSettings.User_AutoLogin && !string.IsNullOrEmpty(BaseService.ApplicationSettings.User_RefreshToken))
            {
                if (Submit_Command.CanExecute(null)) Submit_Command.Execute(null);
            }

            return Task.CompletedTask;
        }

        private void SetLoginVisibility()
        {
            if (LoginVisible)
            {
                LoginVisible = false;
            }
            else
            {
                LoginVisible = true;
            }
        }

        public override string Title { get { return BaseService.Language.GetString("Generic_Login"); } }
        
        /// <summary>
        /// Gets or sets the Usernames property value.
        /// </summary>
        public List<string> Usernames   
        {
            get { return GetValue<List<string>>(); }
            set { SetValue(value); }
        }
        
        /// <summary>
        /// Gets or sets the Username property value.
        /// </summary>
        public string Username
        {
            get { return GetValue<string>(); }
            set
            {
                SetValue(value);
            }
        }

        /// <summary>
        /// Gets or sets the Password property value.
        /// </summary>
        public string Password
        {
            get { return GetValue<string>(); }
            set
            {
                SetValue(value);
            }
        }

        /// <summary>
        /// Gets or sets the AutoLogin property value.
        /// </summary>
        public bool AutoLogin
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Registration_Name property value.
        /// </summary>
        public string Registration_Name
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        //private async Task<bool> Check_Registration_Name(string e)
        //{
        //    bool? result = false;

        //    if (e != null && e != "" && e.Length >= 3)
        //    {
        //        /* Call service asynchronously */
        //        result = await RestService?.CheckIfLicenseIsAvailableAsync(e);

        //        if (result.Value == false)
        //        {
        //            await DialogService.ShowErrorAsync(LanguageService.GetString("EX_LICENSE_NAME"));
        //        }
        //    }
        //    else
        //    {
        //        await DialogService.ShowErrorAsync(LanguageService.GetString("Warning_LicenseNameSize"));
        //    }

        //    return result.Value;
        //}

        /// <summary>
        /// Gets or sets the Registration_Mail property value.
        /// </summary>
        public string Registration_Mail
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        //private async Task<bool> Check_Registration_Mail(string e)
        //{
        //    bool? result = false;

        //    if (e != null && e != "" && NetworkHandler.IsValidEMail(e))
        //    {
        //        result = await RestService?.CheckIfEmailIsAvailableAsync(e);

        //        if (result.Value == false)
        //        {
        //            await DialogService.ShowErrorAsync(LanguageService.GetString("EX_LICENSE_EMAIL"));
        //        }
        //    }
        //    else
        //    {
        //        await DialogService.ShowErrorAsync(LanguageService.GetString("Warning_Invalid_Email"));
        //    }

        //    return result.Value;
        //}

        /// <summary>
        /// Gets or sets the TimeZones property value.
        /// </summary>
        public List<TimeZoneInfo> TimeZones
        {
            get { return GetValue<List<TimeZoneInfo>>(); }
            set { SetValue(value); }
        }
        
        /// <summary>
        /// Gets or sets the Registration_TimeZone property value.
        /// </summary>
        public string Registration_TimeZone
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Registration_Password property value.
        /// </summary>
        public string Registration_Password
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Registration_Password_Check property value.
        /// </summary>
        public string Registration_Password_Check
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Registration_Modules property value.
        /// </summary>
        public List<Module> Registration_Modules
        {
            get { return GetValue<List<Module>>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the ShowLogin property value.
        /// </summary>
        public bool LoginVisible
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }
        
        protected virtual async Task<bool> CheckFields()
        {
            bool result = true;

            if (LoginVisible)
            {
                if (Username is null || Username.Length < 3)
                {
                    await BaseService.Dialog
                        .ShowErrorAsync(BaseService.Language.GetString("Warning_UsernameSize"));
                    result = false;
                }

                if (Password is null || Password.Length < 6)
                {
                    await BaseService.Dialog
                        .ShowErrorAsync(BaseService.Language.GetString("Warning_PasswordSize"));
                    result = false;
                }

                BaseService.ApplicationSettings.User_AutoLogin = AutoLogin;
            }
            else
            {
                if (Registration_Name is null || Registration_Name.Length < 3)
                {
                    await BaseService.Dialog
                        .ShowErrorAsync(BaseService.Language.GetString("Warning_LicenseNameSize"));
                    result = false;
                }

                if (Registration_Mail is null || !Network.IsValidEMail(Registration_Mail))
                {
                    await BaseService.Dialog
                        .ShowErrorAsync(BaseService.Language.GetString("Warning_Invalid_Email"));
                    result = false;
                }

                if(string.IsNullOrEmpty(Registration_TimeZone))
                {
                    await BaseService.Dialog
                        .ShowErrorAsync(BaseService.Language.GetString("Warning_NoTimeZone_Selected"));
                    result = false;
                }

                if (Registration_Password is null || Registration_Password.Length < 6)
                {
                    await BaseService.Dialog
                        .ShowErrorAsync(BaseService.Language.GetString("Warning_PasswordSize"));
                    result = false;
                }

                Match passwordMatch = Regex.Match(Registration_Password, Constants.PasswordRegEx, RegexOptions.None);

                if (!passwordMatch.Success)
                {
                    await BaseService.Dialog
                        .ShowErrorAsync(BaseService.Language.GetString("Warning_PasswordSize"));
                    result = false;
                }

                if (Registration_Password_Check is null || Registration_Password_Check.Length < 6)
                {
                    await BaseService.Dialog
                        .ShowErrorAsync(BaseService.Language.GetString("Warning_PasswordSize"));
                    result = false;
                }

                if (!Registration_Password.Equals(Registration_Password_Check))
                {
                    await BaseService.Dialog
                        .ShowErrorAsync(BaseService.Language.GetString("Warning_PasswordMatch"));
                    result = false;
                }
            }

            return result;
        }

        /// <summary>
        /// Override to implement authentication
        /// </summary>
        /// <example>
        /// <code>
        /// public override Task<bool> AuthenticateAsync()
        /// {
        ///     bool result = false;
        /// 
        ///     if (SettingsService.User_AutoLogin && !string.IsNullOrEmpty(SettingsService.User_RefreshToken))
        ///     {
        ///         await RestService.AuthenticateWithRefreshTokenAsync(SettingsService.User_RefreshToken);
        /// 
        ///         if (Context.CurrentProfile?.Token != null)
        ///         {
        ///             SettingsService.User_AutoLogin = true;
        ///         }
        ///         else
        ///         {
        ///             SettingsService.User_AutoLogin = false;
        ///         }
        ///     }
        ///     else
        ///     {
        ///         if (await CheckFields())
        ///              RestService.AuthenticateWithTokenAsync(Username, Password);
        ///     }
        /// 
        ///     if (Context.CurrentProfile != null && Context.CurrentProfile != null && Context.CurrentProfile.Token != null)
        ///     {
        ///         result = true;
        ///     }
        /// 
        ///     return result;
        /// }
        /// </code>
        /// </example>
        /// <returns>
        /// Context.CurrentProfile != null && Context.CurrentProfile != null && Context.CurrentProfile.Token != null
        /// </returns>
        public abstract Task<bool> AuthenticateAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <example>
        /// <code>
        /// public override async Task<bool> RegisterAsync()
        /// {
        ///     bool result = false;
        /// 
        ///     if (await CheckFields() && await CheckRegistrationIsAvailable())
        ///     {
        ///         try
        ///         {
        ///             await SynergyService.BusyService.StartBusyAsync();
        /// 
        ///             result = await RestService?.RegisterExternal(new RegistrationData
        ///             {
        ///                 ApplicationId = 1,
        ///                 LicenseName = Registration_Name,
        ///                 Email = Registration_Mail,
        ///                 Password = Registration_Password,
        ///                 Modules = Registration_Modules,
        ///                 UsersAllowed = 1,
        ///                 TimeZoneId = Registration_TimeZone
        ///             });
        /// 
        ///             if (result)
        ///             {
        ///                 await DialogService
        ///                     .ShowInformationAsync(LanguageService.GetString("Warning_Registration_ConfirmEmail"));
        /// 
        ///                 Username = Registration_Mail;
        ///                 LoginVisible = true;
        ///             }
        ///         }
        ///         finally
        ///         {
        ///             await SynergyService.BusyService.EndBusyAsync();
        ///         }
        ///     }
        /// 
        ///     return result;
        /// }
        /// </code>
        /// </example>
        /// <returns></returns>
        public abstract Task<bool> RegisterAsync();


        /// <summary>
        /// 
        /// </summary>
        /// <example>
        /// <code>
        /// public override async Task ForgotPasswordAsync()
        /// {
        ///     var result = await UIVisualizerService.ShowDialogAsync(
        ///         typeof(IForgotPasswordWindow),
        ///         new ForgotPasswordViewModel(
        ///             Context, 
        ///             Busy,
        ///             LanguageService,
        ///             SettingsService,
        ///             TelemetryService,
        ///             DialogService));
        /// 
        ///     if (result.HasValue && result.Value)
        ///     {
        ///         await DialogService
        ///                 .ShowInformationAsync(LanguageService.GetString("Warning_Reset_Password"));
        ///     }
        /// }
        /// </code>
        /// </example>
        /// <returns></returns>
        public abstract Task<bool> ForgotPasswordAsync();

        //private async Task<bool> CheckRegistrationIsAvailable()
        //{
        //    if (await Check_Registration_Name(Registration_Name) &&
        //        await Check_Registration_Mail(Registration_Mail) &&
        //        Registration_Password_Check != null && Registration_Password != null &&
        //        Registration_Password_Check.Equals(Registration_Password) &&
        //        Regex.IsMatch(Registration_Password, Constants.PasswordRegEx))
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}
        

        public override Task OnSubmittanceAsync(OnSubmittanceMessage e)
        {
            if (!e.Handled)
            {
                if (BaseService.Navigation.CanGoBack)
                    BaseService.Navigation.GoBack();

                e.Handled = true;
            }

            return Task.CompletedTask;
        }

        public override Task OnCancellationAsync(OnCancellationMessage e)
        {
            if (!e.Handled)
            {
                IsCancelled = true;

                if (BaseService.Navigation.CanGoBack)
                    BaseService.Navigation.GoBack();

                e.Handled = true;
            }

            return Task.CompletedTask;
        }
    }
}