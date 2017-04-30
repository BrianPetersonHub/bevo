using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;

namespace bevo.Models
{
    public class EditUserViewModel
    {
        [Required(ErrorMessage = "First Name is a Required field.")]
        [Display(Order = 1, Name = "First Name")]
        public String FirstName { get; set; }

        [Display(Order = 2, Name = "Middle Initial")]
        public String MiddleInitial { get; set; }

        [Required(ErrorMessage = "Last Name is a Required field.")]
        [Display(Order = 3, Name = "Last Name")]
        public String LastName { get; set; }

        [Required(ErrorMessage = "Street is a Required field.")]
        [Display(Order = 4, Name = "Street Address")]
        public String Street { get; set; }

        [Required(ErrorMessage = "City is a Required field.")]
        [RegularExpression("^((?!^City$)[a-zA-Z '])+$")]
        [Display(Order = 5, Name = "City")]
        public String City { get; set; }

        [Required(ErrorMessage = "State is a Required field.")]
        [DataType(DataType.Text)]
        [Display(Order = 6, Name = "State")]
        public String State { get; set; }

        [Required(ErrorMessage = "Zipcode is a Required field.")]
        [RegularExpression("\\d{5}", ErrorMessage = "Zipcode must be five digits long")]
        [Display(Order = 7, Name = "Zipcode")]
        public String ZipCode { get; set; }

        [Required(ErrorMessage = "Date of Birth is a Required field.")]
        [Display(Order = 10, Name = "Date of Birth")]
        //[DataType(DataType.Date)]
        public String Birthday { get; set; }

        public String Email { get; set; }

        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid Phone number")]
        public String PhoneNumber { get; set; }
    }
}