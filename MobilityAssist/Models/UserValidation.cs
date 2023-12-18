using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MobilityAssist.Models
{
    [MetadataType(typeof(UserMetaData))]
    public partial class User
    {
        [Bind(Exclude ="user_id")]
        public class UserMetaData
        {
            [ScaffoldColumn(false)]
            public int user_id { get; set; }

            [DisplayName("First Name")]
            [Required(ErrorMessage ="Треба ввести ім'я")]
            [StringLength(20)]
            public string first_name { get; set; }
            [DisplayName("Email")]
            [UniqueEmail(ErrorMessage = "Користувач з такою поштою вже існує")]
            [Required(ErrorMessage = "Треба ввести пошту")]
            [StringLength(30)]
            [EmailAddress(ErrorMessage ="Введіть дійсну пошту")]
            public string email { get; set; }

            [DisplayName("Пароль")]
            [Required(ErrorMessage = "Треба ввести пароль")]
            [MinLength(5, ErrorMessage ="Пароль занадто малий")]
            public string password { get; set; }
        }
    }
    public class UniqueEmailAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string email = value.ToString();
            using (MobilityAssistEntities db = new MobilityAssistEntities()) {
                if (db.Users.Any(u => u.email == email))
                {
                    return new ValidationResult(ErrorMessage);
                }
                return ValidationResult.Success;
            }            
        }
    }
    
}