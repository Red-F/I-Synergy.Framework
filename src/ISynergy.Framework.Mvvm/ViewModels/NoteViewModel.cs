﻿using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using Microsoft.Extensions.Logging;
using ISynergy.Framework.Mvvm.Events;
using ISynergy.Framework.Core.Attributes;

namespace ISynergy.Framework.Mvvm.ViewModels
{
    /// <summary>
    /// Class NoteViewModel.
    /// Implements the <see cref="ViewModelDialog{String}" />
    /// </summary>
    /// <seealso cref="ViewModelDialog{String}" />
    public class NoteViewModel : ViewModelDialog<string>
    {
        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>The title.</value>
        public override string Title
        {
            get
            {
                return BaseCommonServices.LanguageService.GetString("Note");
            }
        }

        /// <summary>
        /// The target property
        /// </summary>
        private readonly string _targetProperty;

        /// <summary>
        /// Initializes a new instance of the <see cref="NoteViewModel"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="commonServices">The common services.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="note">The note.</param>
        [PreferredConstructor]
        public NoteViewModel(
            IContext context,
            IBaseCommonServices commonServices,
            ILoggerFactory loggerFactory,
            string note)
            : base(context, commonServices, loggerFactory)
        {
            SelectedItem = note;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NoteViewModel"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="commonServices">The common services.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="note">The note.</param>
        /// <param name="targetProperty">The target property.</param>
        public NoteViewModel(
            IContext context,
            IBaseCommonServices commonServices,
            ILoggerFactory loggerFactory,
            string note,
            string targetProperty)
            : this(context, commonServices, loggerFactory, note)
        {
            _targetProperty = targetProperty;
        }

        /// <summary>
        /// Called when [submitted].
        /// </summary>
        /// <param name="e">The e.</param>
        protected override void OnSubmitted(SubmitEventArgs<string> e)
        {
            if(!string.IsNullOrEmpty(_targetProperty))
            {
                base.OnSubmitted(new SubmitEventArgs<string>(e.Owner, e.Result, _targetProperty));
            }
            else
            {
                base.OnSubmitted(e);
            }
        }
    }
}
