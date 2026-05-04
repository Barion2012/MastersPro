using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Agregator.Data
{
    public partial class Vacancy
    {
        public Vacancy()
        {
            Feedbacks = new HashSet<Feedback>();
            VacancyBuildObjectCts = new HashSet<VacancyBuildObjectCt>();
        }

        public int RowId { get; set; }
        public int? Position { get; set; }
        public string BillingTypeIdValue { get; set; }
        public string BillingTypeIdView { get; set; }
        public string AreaIdValue { get; set; }
        public string AreaIdView { get; set; }
        public string Code { get; set; }
//        public string Name { get; set; }
 //       public string Description { get; set; }
        public string SalaryCurrencyValue { get; set; }
        public string SalaryCurrencyView { get; set; }
        public string SpecializationsIdValue { get; set; }
        public string SpecializationsIdView { get; set; }
        public string TypeIdValue { get; set; }
        public string TypeIdView { get; set; }
        public string KeySkillsNameValue { get; set; }
        public string KeySkillsNameView { get; set; }
        public decimal? SalaryFrom { get; set; }
        public decimal? SalaryTo { get; set; }
        public bool? AddressShowMetroOnly { get; set; }

        public string DescriptionTab1 { get; set; }

        public string DescriptionTab2 { get; set; }

        public string DescriptionTab3 { get; set; }


        public virtual Position PositionNavigation { get; set; }
        public virtual ICollection<Feedback> Feedbacks { get; set; }
        public virtual ICollection<VacancyBuildObjectCt> VacancyBuildObjectCts { get; set; }

        [NotMapped]
        public virtual int[] objects { set; get; }


        public Guid Creator { set; get; }
        public Guid? Updater { set; get; }
        public DateTime Created { set; get; }
        public DateTime? Updated { set; get; }

    }
}
