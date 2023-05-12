using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Models
{
    public class CountryDTO : CountryCreationDTO
    {
        public int Id { get; set; }
        
        public IList<HotelDTO> Hotels { get; set; }
    }
}