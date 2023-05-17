using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Models
{
    public class CountryCreationDTO
    {
        [Required]
        [StringLength(maximumLength: 50, ErrorMessage = "Country name is too long!")]
        public string Name { get; set; }
        [Required]
        [StringLength(maximumLength: 3, ErrorMessage = "Short country name is too long!")]
        public string ShortName { get; set; }
    }
}