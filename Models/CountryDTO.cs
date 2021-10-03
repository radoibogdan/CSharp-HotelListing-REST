using HotelListing.Controllers.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListing.Models
{
    // Non Id required operations => Creating or Updating or looking at details
    public class CreateCountryDTO
    {
        [Required]
        [StringLength(maximumLength: 50, ErrorMessage = "Country name is too long.")]
        public string Name { get; set; }
        [Required]
        [StringLength(maximumLength: 2, ErrorMessage = "Country shortname is too long.")]
        public string ShortName { get; set; }
    }
    
    // Inherits from CreateCountryDTO. For Update, looking at details
    public class CountryDTO : CreateCountryDTO
    {
        public int Id { get; set; }
        public IList<HotelDTO> Hotels { get; set; }
    }

    
}
