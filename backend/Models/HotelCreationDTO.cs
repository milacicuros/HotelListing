using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Models
{
    public class HotelCreationDTO
    {
        [Required]
        [StringLength(maximumLength: 150, ErrorMessage = "Hotel name is too long!")]
        public string Name { get; set; }
        
        [Required]
        [StringLength(maximumLength: 250, ErrorMessage = "Address name is too long!")]
        public string Address { get; set; }
        
        [Required]
        [Range(1,5)]
        public double Rating { get; set; }
        
        [Required]
        public double CountryId { get; set; }
    }
}