﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Data;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.Enumerations;
using Microsoft.Extensions.Logging;

namespace ISynergy.Framework.Mvvm.ViewModels
{
    /// <summary>
    /// Class SelectionViewModel.
    /// Implements the <see name="ViewModelBlade{ObservableCollection{object}}" />
    /// Implements the <see cref="IViewModelBlade" />
    /// </summary>
    /// <seealso name="ViewModelBlade{ObservableCollection{object}}" />
    /// <seealso cref="IViewModelBlade" />
    public class SelectionViewModel : ViewModelBlade<ObservableCollection<object>>, IViewModelBlade
    {
        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>The title.</value>
        public override string Title { get { return BaseCommonServices.LanguageService.GetString("Selection"); } }

        /// <summary>
        /// Gets or sets the raw items.
        /// </summary>
        /// <value>The raw items.</value>
        private IEnumerable<object> RawItems { get; set; }

        /// <summary>
        /// Gets or sets the SelectionMode property value.
        /// </summary>
        /// <value>The selection mode.</value>
        public SelectionModes SelectionMode
        {
            get { return GetValue<SelectionModes>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Item property value.
        /// </summary>
        /// <value>The items.</value>
        public ObservableCollection<object> Items
        {
            get { return GetValue<ObservableCollection<object>>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Query property value.
        /// </summary>
        /// <value>The query.</value>
        public string Query
        {
            get { return GetValue<string>(); }
            set
            {
                SetValue(value);
                QueryItems();
            }
        }

        /// <summary>
        /// Gets or sets the search command.
        /// </summary>
        /// <value>The search command.</value>
        public RelayCommand Search_Command { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectionViewModel"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="commonServices">The common services.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="items">The items.</param>
        /// <param name="selectedItems">The selected items.</param>
        /// <param name="selectionMode">The selection mode.</param>
        public SelectionViewModel(
            IContext context,
            IBaseCommonServices commonServices,
            ILoggerFactory loggerFactory,
            IEnumerable<IModelBase> items,
            IEnumerable<IModelBase> selectedItems,
            SelectionModes selectionMode = SelectionModes.Single)
            : base(context, commonServices, loggerFactory)
        {
            SelectionMode = selectionMode;

            Validator = new Action<IObservableClass>(arg =>
            {
                if (SelectionMode == SelectionModes.Single && SelectedItem.Count != 1)
                {
                    Properties[nameof(SelectedItem)].Errors.Add(commonServices.LanguageService.GetString("Warning_Select_Item"));
                }

                if (SelectionMode == SelectionModes.Multiple && SelectedItem.Count < 1)
                {
                    Properties[nameof(SelectedItem)].Errors.Add(commonServices.LanguageService.GetString("Warning_Select_Item"));
                }
            });

            Query = string.Empty;
            RawItems = items;
            Items = new ObservableCollection<object>(items);
            SelectedItem = new ObservableCollection<object>(selectedItems);
        }

        /// <summary>
        /// Queries the items.
        /// </summary>
        private void QueryItems()
        {
            if (IsInitialized && RawItems != null)
            {
                if (string.IsNullOrEmpty(Query) || Query.Trim() == "*")
                {
                    Items = new ObservableCollection<object>(RawItems);
                }
                else
                {
                    var filteredList = new List<object>();

                    foreach (var item in RawItems)
                    {
                        foreach (var prop in item.GetType().GetProperties().Select(s => s.GetValue(item)).ToList())
                        {
                            if (prop is string value && !string.IsNullOrEmpty(value) && value.IndexOf(Query, StringComparison.OrdinalIgnoreCase) != -1)
                            {
                                filteredList.Add(item);
                                break;
                            }
                        }
                    }

                    Items = new ObservableCollection<object>(filteredList);
                }
            }
        }
    }
}
