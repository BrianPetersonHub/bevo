using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace bevo.Models
{
    public enum PayeeType
    {
        Credit_Card,
        Utilities,
        Rent,
        Mortgage,
        Other
    }
    public class Payee
    {
        public Int32 PayeeID { get; set; }

        [DataType(DataType.Text)]
        [Required(ErrorMessage = "Please enter Payee Name")]
        [Display(Order = 1, Name = "Payee Name")]
        public String Name { get; set; }

        [Required(ErrorMessage = "Street is a Required field.")]
        [RegularExpression("^((?!^Address$)[0-9A-Za-z #.,])+$")]
        [Display(Order = 2, Name = "Street Address")]
        public String Street { get; set; }

        [Required(ErrorMessage = "City is a Required field.")]
        [RegularExpression("^((?!^City$)[a-zA-Z '])+$")]
        [Display(Order = 3, Name = "City")]
        public String City { get; set; }

        [Required(ErrorMessage = "State is a Required field.")]
        [DataType(DataType.Text)]
        [Display(Order = 4, Name = "State")]
        public String State { get; set; }

        [Required(ErrorMessage = "Zipcode is a Required field.")]
        [RegularExpression("\\d{5}", ErrorMessage = "Zipcode must be five digits long")]
        [Display(Order = 5, Name = "Zipcode")]
        public String Zipcode { get; set; }

        [Required(ErrorMessage = "Phone Number is a Required field.")]
        [Display(Order = 6, Name = "Phone Number")]
        [RegularExpression("^[01]?[- .]?\\(?[2-9]\\d{2}\\)?[- .]?\\d{3}[- .]?\\d{4}$",
            ErrorMessage = "Phone must be a valid format")]
        public String PhoneNumber { get; set; }

        [Required(ErrorMessage = "Please enter Payee Type")]
        [Display(Order = 7, Name = "Payee Type")]
        public PayeeType PayeeType { get; set; }

        // Navigational
        public virtual List<AppUser> AppUsers { get; set; }
    }
}