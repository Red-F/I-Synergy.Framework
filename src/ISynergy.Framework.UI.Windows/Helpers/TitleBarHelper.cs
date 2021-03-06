﻿using System.ComponentModel;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml;

namespace ISynergy.Framework.UI.Helpers
{
    /// <summary>
    /// Class TitleBarHelper.
    /// Implements the <see cref="INotifyPropertyChanged" />
    /// </summary>
    /// <seealso cref="INotifyPropertyChanged" />
    public class TitleBarHelper : INotifyPropertyChanged
    {
        /// <summary>
        /// The core title bar
        /// </summary>
        private static CoreApplicationViewTitleBar _coreTitleBar;
        /// <summary>
        /// The title position
        /// </summary>
        private Thickness _titlePosition;
        /// <summary>
        /// The title visibility
        /// </summary>
        private Visibility _titleVisibility;

        /// <summary>
        /// Initializes a new instance of the <see cref="TitleBarHelper" /> class.
        /// </summary>
        public TitleBarHelper()
        {
            _coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            _coreTitleBar.LayoutMetricsChanged += CoreTitleBar_LayoutMetricsChanged;
            _titlePosition = CalculateTilebarOffset(_coreTitleBar.SystemOverlayLeftInset, _coreTitleBar.Height);
            _titleVisibility = Visibility.Visible;
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        /// <returns></returns>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static TitleBarHelper Instance { get; } = new TitleBarHelper();

        /// <summary>
        /// Gets the title bar.
        /// </summary>
        /// <value>The title bar.</value>
        public CoreApplicationViewTitleBar TitleBar
        {
            get
            {
                return _coreTitleBar;
            }
        }

        /// <summary>
        /// Gets or sets the title position.
        /// </summary>
        /// <value>The title position.</value>
        public Thickness TitlePosition
        {
            get
            {
                return _titlePosition;
            }
            set
            {
                if (value.Left != _titlePosition.Left || value.Top != _titlePosition.Top)
                {
                    _titlePosition = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TitlePosition)));
                }
            }
        }

        /// <summary>
        /// Gets or sets the title visibility.
        /// </summary>
        /// <value>The title visibility.</value>
        public Visibility TitleVisibility
        {
            get
            {
                return _titleVisibility;
            }
            set
            {
                _titleVisibility = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TitleVisibility)));
            }
        }

        /// <summary>
        /// Exits the fullscreen.
        /// </summary>
        public void ExitFullscreen()
        {
            TitleVisibility = Visibility.Visible;
        }

        /// <summary>
        /// Goes the fullscreen.
        /// </summary>
        public void GoFullscreen()
        {
            TitleVisibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Cores the title bar layout metrics changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The arguments.</param>
        private void CoreTitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        {
            TitlePosition = CalculateTilebarOffset(_coreTitleBar.SystemOverlayLeftInset, _coreTitleBar.Height);
        }

        /// <summary>
        /// Calculates the tilebar offset.
        /// </summary>
        /// <param name="leftPosition">The left position.</param>
        /// <param name="height">The height.</param>
        /// <returns>Thickness.</returns>
        private static Thickness CalculateTilebarOffset(double leftPosition, double height)
        {
            // top position should be 6 pixels for a 32 pixel high titlebar hence scale by actual height
            var correctHeight = height / 32 * 6;

            return new Thickness(leftPosition + 12, correctHeight, 0, 0);
        }
    }
}
