﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ISynergy.Services
{
    public class ConverterService : IConverterService
    {
        private readonly IContext Context;

        public ConverterService(IContext context)
        {
            Context = context;
        }

        public int ConvertMediaColor2Integer(object mediacolor)
        {
            throw new NotImplementedException();
        }

        public string ConvertDecimalToCurrency(decimal value)
        {
            string currencySymbol = "$";

            currencySymbol = Context.CurrencySymbol;

            NumberFormatInfo info = Thread.CurrentThread.CurrentCulture.NumberFormat;
            info.CurrencySymbol = $"{currencySymbol} ";
            info.CurrencyNegativePattern = 1;

            return string.Format(info, "{0:C2}", value).Replace("  ", " ");
        }
    }
}
