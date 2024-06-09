/****************************************************
 * Program Name: Anniversary.cs
 * Author(s): Ethan Rivers
 * Date: June 2, 2024
 * Description: Class definitions for Anniversary and Address objects
 * Version: 1.0
 ****************************************************/
using Newtonsoft.Json;
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
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? StreetAddress { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? Municipality { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? Province { get; set; } // see schema for accepted provinces

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? PostalCode { get; set; }
    }

    public class Anniversary
    {
        [Required]
        public string Names { get; set; } = "";

        [Required]
        public string AnniversaryDate { get; set; } = "";

        [Required]
        public string AnniversaryType { get; set; } = "";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? Description { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? Email { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? Phone { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Address? Address { get; set; }
    }

}
