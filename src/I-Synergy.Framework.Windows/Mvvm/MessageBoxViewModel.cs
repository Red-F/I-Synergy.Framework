﻿using ISynergy.Enumerations;
using ISynergy.Mvvm;
using ISynergy.Services;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace ISynergy.ViewModels.Library
{
    public class MessageBoxViewModel : ViewModelMessageBox<object>
    {
        /// <summary>
        /// Gets or sets the MessageImageSource property value.
        /// </summary>
        public ImageSource MessageImageSource
        {
            get { return GetValue<ImageSource>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the ContentFlowDirection property value.
        /// </summary>
        public FlowDirection ContentFlowDirection
        {
            get { return GetValue<FlowDirection>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the TitleFlowDirection property value.
        /// </summary>
        public FlowDirection TitleFlowDirection
        {
            get { return GetValue<FlowDirection>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the ContentTextAlignment property value.
        /// </summary>
        public TextAlignment ContentTextAlignment
        {
            get { return GetValue<TextAlignment>(); }
            set { SetValue(value); }
        }

        public MessageBoxViewModel(
            IContext context,
            IBaseService synergyService,
            string message,
            string title,
            MessageBoxButton button = MessageBoxButton.OK,
            MessageBoxImage image = MessageBoxImage.Information,
            MessageBoxResult result = MessageBoxResult.OK)
        : base(context, synergyService, message, title, button, image, result)
        {
            SetButtonVisibility(button);
            SetImageSource(image);
            SetButtonDefault(result);
        }

        private void SetButtonDefault(MessageBoxResult defaultResult)
        {
            switch (defaultResult)
            {
                case MessageBoxResult.OK:
                    IsOkDefault = true;
                    break;

                case MessageBoxResult.Yes:
                    IsYesDefault = true;
                    break;

                case MessageBoxResult.No:
                    IsNoDefault = true;
                    break;

                case MessageBoxResult.Cancel:
                    IsCancelDefault = true;
                    break;
            }
        }

        private void SetButtonVisibility(MessageBoxButton buttonOption)
        {
            switch (buttonOption)
            {
                case MessageBoxButton.YesNo:
                    OkVisible = false;
                    CancelVisible = false;
                    YesNoVisible = true;
                    break;

                case MessageBoxButton.YesNoCancel:
                    OkVisible = false;
                    CancelVisible = true;
                    YesNoVisible = true;

                    break;

                case MessageBoxButton.OK:
                    OkVisible = true;
                    CancelVisible = false;
                    YesNoVisible = false;

                    break;

                case MessageBoxButton.OKCancel:
                    OkVisible = true;
                    CancelVisible = true;
                    YesNoVisible = false;

                    break;

                default:
                    OkVisible = false;
                    CancelVisible = false;
                    YesNoVisible = false;

                    break;
            }
        }

        private void SetImageSource(MessageBoxImage image)
        {
            switch (Convert.ToInt32(image))
            {
                case 16:
                    MessageImageSource = new BitmapImage(new Uri("/I-Synergy.Framework.Windows;component/Assets/Images/Error.png", UriKind.Relative));
                    break;

                case 48:
                    MessageImageSource = new BitmapImage(new Uri("/I-Synergy.Framework.Windows;component/Assets/Images/Warning.png", UriKind.Relative));
                    break;

                case 32:
                    MessageImageSource = new BitmapImage(new Uri("/I-Synergy.Framework.Windows;component/Assets/Images/Question.png", UriKind.Relative));
                    break;

                case 64:
                    MessageImageSource = new BitmapImage(new Uri("/I-Synergy.Framework.Windows;component/Assets/Images/Information.png", UriKind.Relative));
                    break;

                default:
                    MessageImageSource = null;
                    break;
            }
        }
    }
}