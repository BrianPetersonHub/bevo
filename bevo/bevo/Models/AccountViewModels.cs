using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace bevo.Models
{
   
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "First Name is a Required field.")]
        [Display(Order = 1, Name = "First Name")]
        public String FirstName { get; set; }

        [Display(Order = 2, Name = "Middle Initial")]
        public Char? MiddleInitial { get; set; }

        [Required(ErrorMessage = "Last Name is a Required field.")]
        [Display(Order = 3, Name = "Last Name")]
        public String LastName { get; set; }

        [Required(ErrorMessage = "Street is a Required field.")]
        [RegularExpression("^((?!^Address$)[0-9A-Za-z #.,])+$")]
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
        [DataType(DataType.Date)]
        public String Birthday { get; set; }

        [Required(ErrorMessage = "PhoneNumber is a required field")]
        [Display(Name = "Phone Number")]
        public String PhoneNumber { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [RegularExpression("^(0|[1-9][0-9]*)$", ErrorMessage ="Invalid Year")]
        [Display(Name ="Birth Year")]
        public string BirthYear { get; set; }
    }
}
