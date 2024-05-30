using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnniversaryMinder
{
    public class Address
    {
        public string? StreetAddress { get; set; }
        public string? Municipality { get; set; }
        public string? Province { get; set; } // see schema for accepted provinces
        public string? PostalCode { get; set; }
    }

    public class Anniversary
    {
        [Required]
        public string Names { get; set; } = ""; 
        [Required]
        public DateOnly AnniversaryDate { get; set; } 
        [Required]
        public string AnniversaryType { get; set; } = ""; 
        public string? Description { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public Address? Address { get; set; }
    }

}
