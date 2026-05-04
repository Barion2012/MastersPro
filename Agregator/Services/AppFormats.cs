using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;

namespace Agregator.Services
{
    public class AppFormats : CultureInfo
    {
        public readonly string LongDatePattern = "dd MMMM yyyy г.";
        public readonly string ShortDatePattern = "dd.MM.yyyy";
        public AppFormats() : base("ru-Ru", true)
        {
            this.NumberFormat.NumberDecimalSeparator = ".";
            this.DateTimeFormat.LongDatePattern = "dd MMMM yyyy г.";
            
            this.DateTimeFormat.ShortDatePattern = "dd.MM.yy";
        }
        //public static AppFormats Instance { get => new AppFormats(); }

    }
}
