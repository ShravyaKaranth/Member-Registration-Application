using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MemberRegistrationAppln.Models
{
    public class LoginDetails
    {
        [Required]
        public string EmployeeName { get; set; }
        [Required, DisplayName("Password")]
        public string Passwrd { get; set; }
        public string authorziedEmployee { get; set; }
      
    }
}