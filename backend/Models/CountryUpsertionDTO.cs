using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Models
{
    public class CountryUpsertionDTO : CountryCreationDTO
    {
        public IList<HotelCreationDTO> Hotels { get; set; }
    }
}