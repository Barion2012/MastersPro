using System;
using System.Collections.Generic;

namespace Agregator.Data
{
    public partial class Country
    {
        public int Id { get; set; }
        public string CountryName { get; set; }
        public string Alfa2 { get; set; }
        public string Alfa3 { get; set; }
        public string FullName { get; set; }
        public sbyte? IsEaes { get; set; }
        public sbyte? UseProfile { get; set; }
    }
}
