using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVCWeatherghan.Models
{
    public class ContactForm
    {
        [Required(ErrorMessage="First Name is required.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required.")]
        public string LastName { get; set; }

        [Required(ErrorMessage="A valid email is required.")]
        public string Email { get; set; }

        [Required(ErrorMessage="A message is required.")]
        public string Message { get; set; }
    }
}