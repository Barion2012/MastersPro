using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Agregator.Models
{
    using Data;
    public class ProfileModel
    {

        [Display(Name = "Почта")]
        public string UserName { get; set; }


        [Display(Name = "Фото")]
        public IFormFile Photo { get; set; }
        public byte[] Face { get; set; }


        [Required(ErrorMessage = RussianIdentityErrorDescriber.DefaultErrorMessage)]
        [DataType(DataType.Text)]
        [Display(Name = "Фамилия")]
        public string Surname { get; set; }
        [Required(ErrorMessage = RussianIdentityErrorDescriber.DefaultErrorMessage)]
        [DataType(DataType.Text)]
        [Display(Name = "Имя")]
        public string Name { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Отчество")]
        public string Middlename { get; set; }  

        [Required(ErrorMessage = RussianIdentityErrorDescriber.DefaultErrorMessage)]
        [DataType(DataType.Date)]
        [Display(Name = "Дата рождения")]
        public DateTime? Birthday { get; set; }
        /*
        [Required(ErrorMessage = defaultErrorMessage)]
        [DataType(DataType.Text)]
        [Display(Name = "Место рождения")]
        public string PlaceOfBirth { get; set; }
        */
        
        
        [Phone]
        [Display(Name = "Номер телефона")]
        [Required(ErrorMessage = RussianIdentityErrorDescriber.DefaultErrorMessage)]

        public string PhoneNumber { get; set; }

        //   [Required(ErrorMessage = defaultErrorMessage)]
        //   [DataType(DataType.Text)]
        [Display(Name = "Пол")]
        public string Male { get; set; }
        public string Female { get; set; }

        //      [Required(ErrorMessage = defaultErrorMessage)]
        [DataType(DataType.Text)]
        [Display(Name = "Гражданство")]
        public int Citizenship { get; set; }
        public IList<Country> Countries { get; set; }
        public bool phoneNumberConfirmed { get; set; }

        public bool agreePrivacy { get; set; }
        public bool agreeRegFNS { get; set; }
    }
}