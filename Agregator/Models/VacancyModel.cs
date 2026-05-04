using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Agregator.Models
{
    using Data;


    public class VacancyListModel : List<VacancyModel>
    {
        public bool profileReady { set; get; }
    }

    public class AccordPage
    {
        public string HtmlTitle { set; get; }
        public string HtmlBody { set; get; }
    }

    public class VacancyModel
    {
        public BuildObject techObject { set; get; }
        public Vacancy vacancy { set; get; }

        public IList<AccordPage> accordPages = new List<AccordPage>();

        public int? fideBackStatus { set; get; }

        public int row { set; get; }
        public int col { set; get; }

        public bool profileReady { set; get; }


    }
}