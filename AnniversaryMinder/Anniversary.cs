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
        public DateOnly AnniversaryDate { get; set; }

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
