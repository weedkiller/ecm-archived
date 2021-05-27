using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Resources;

namespace DansLesGolfs.Models
{
    public class RegisterModel
    {
        [DataType(DataType.EmailAddress)]
        [DisplayName("Email")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationStrings))]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [DisplayName("Old Password")]
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationStrings))]
        public string OldPassword { get; set; }

        [DataType(DataType.Password)]
        [DisplayName("Password")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationStrings))]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [DisplayName("Confirm Password")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationStrings))]
        public string ConfirmPassword { get; set; }

        [DataType(DataType.Text)]
        [DisplayName("First Name")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationStrings))]
        public string FirstName { get; set; }

        [DataType(DataType.Text)]
        [DisplayName("Last Name")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationStrings))]
        public string LastName { get; set; }

        [DisplayName("Receive newsletters")]
        public bool IsReceiveEmailInfo { get; set; }
    }
}