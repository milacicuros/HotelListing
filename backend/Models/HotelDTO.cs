using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Models
{
    public class HotelDTO : HotelCreationDTO
    {
        public int Id { get; set; }
        public CountryDTO Country { get; set; }
    }
}