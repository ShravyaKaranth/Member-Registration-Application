using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MemberRegistrationAppln.Models
{
    public class HouseholdMember
    {
       
        public int ApplicationID { get; set; }
        [Required]
        public string FirstName { get; set; }
        public string MiddleInitial { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Suffix { get; set; }
        [Required]
        public string DOB { get; set; }
        [Required]
        public string Gender { get; set; }
        public int Employeeid { get; set; }
        public List<HouseholdMember> MembersList { get; set; }
        public string Relationship { get; set; }
        public List<string> RelationshipMembers { get; set; }

    }

}