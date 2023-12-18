using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Drawing;

namespace MobilityAssist.Models
{
    //[MetadataType(typeof(AddressMetaData))]
    //public partial class Address
    //{
    //    //[Bind(Exclude = "address_id")]
    //    public class AddressMetaData
    //    {
    //        [ScaffoldColumn(false)]
    //        public int address_id { get; set; }

    //        [DisplayName("Адреса")]
    //        [Required(ErrorMessage = "Треба ввести адресу")]
    //        [UniqueAddress(ErrorMessage = "Вже є така адреса")]
    //        public string address_numb { get; set; }

    //        [DisplayName("Координата Х")]
    //        [Required(ErrorMessage = "Треба ввести координату х")]
    //        [UniqueCoordsX(ErrorMessage = "Вже є такі координати")]            
    //        public double address_coordx { get; set; }
    //        [DisplayName("Координата У")]
    //        [Required(ErrorMessage = "Треба ввести координату у")]
    //        [UniqueCoordsY(ErrorMessage = "Вже є такі координати")]            
    //        public double address_coordy { get; set; }
    //    }
    //}
    //public class UniqueAddressAttribute : ValidationAttribute
    //{
    //    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    //    {
    //        string address_numb = value.ToString();
    //        using (MobilityAssistEntities db = new MobilityAssistEntities())
    //        {
    //            var currentAddress = (Address)validationContext.ObjectInstance;
    //            if (db.Addresses.Where(u => u.street_id + u.address_numb == u.street_id + address_numb).Any(u => u.address_id != currentAddress.address_id))
    //            {
    //                return new ValidationResult(ErrorMessage);
    //            }
    //            return ValidationResult.Success;
    //        }
    //    }
    //}
    //public class UniqueCoordsXAttribute : ValidationAttribute
    //{
    //    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    //    {
    //        string coordx = value.ToString();
    //        using (MobilityAssistEntities db = new MobilityAssistEntities())
    //        {
    //            var currentAddress = (Address)validationContext.ObjectInstance;
    //            if (db.Addresses.Where(u => u.address_coordx.ToString() + u.address_coordy.ToString() ==  coordx + u.address_coordy.ToString()).Any(u => u.address_id != currentAddress.address_id))
    //            {
    //                return new ValidationResult(ErrorMessage);
    //            }
    //            return ValidationResult.Success;
    //        }
    //    }
    //}
    //public class UniqueCoordsYAttribute : ValidationAttribute
    //{
    //    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    //    {
    //        string coordy = value.ToString();
    //        using (MobilityAssistEntities db = new MobilityAssistEntities())
    //        {
    //            var currentAddress = (Address)validationContext.ObjectInstance;
    //            if (db.Addresses.Where(u => u.address_coordx.ToString() + u.address_coordy.ToString() == u.address_coordy.ToString() + coordy).Any(u => u.address_id != currentAddress.address_id))
    //            {
    //                return new ValidationResult(ErrorMessage);
    //            }
    //            return ValidationResult.Success;
    //        }
    //    }
    //}
}