using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace bevo.Models
{

    public class Person
    {
        public Int32 PersonID { get; set; }

        public bool Enabled { get; set; }

        [Required(ErrorMessage = "First Name is a Required field.")]
        [Display(Order = 1, Name = "First Name")]
        public String FirstName { get; set; }

        [Display(Order = 2, Name = "Middle Initial")]
        public Char MiddleInitial { get; set; }

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

        [Required(ErrorMessage = "Email is a Required field.")]
        [DataType(DataType.EmailAddress)]
        [Display(Order = 8, Name = "Email Address")]
        [RegularExpression("^[A-Za-z0-9._%+-]*@[A-Za-z0-9.-]*\\.[A-Za-z0-9-]{2,}$",
            ErrorMessage = "Email must be properly formatted.")]
        public String Email { get; set; }

        [Required(ErrorMessage = "Phone Number is a Required field.")]
        [Display(Order = 9, Name = "Phone Number")]
        [RegularExpression("^[01]?[- .]?\\(?[2-9]\\d{2}\\)?[- .]?\\d{3}[- .]?\\d{4}$", 
            ErrorMessage = "Phone must be a valid format")]
        public String PhoneNumber { get; set; }

        [Required(ErrorMessage = "Date of Birth is a Required field.")]
        [Display(Order = 10, Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public String Birthday { get; set; }

    
        [Required(ErrorMessage = "Password is a Required field.")]
        [Display(Order = 11, Name = "Password")]
        public String Password { get; set; }

        // Navigational 
        public virtual List<CheckingAccount> CheckingAccounts { get; set; }
        public virtual List<SavingAccount> SavingAccounts { get; set; }
        public virtual IRAccount IRAccount { get; set; }
        public virtual StockPortfolio StockPortfolio { get; set; }
        public virtual List<Payee> Payees { get; set; }
    }
}