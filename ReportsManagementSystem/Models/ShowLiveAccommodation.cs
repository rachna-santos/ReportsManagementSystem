using System.ComponentModel.DataAnnotations;

namespace ReportsManagementSystem.Models
{
    public class ShowLiveAccommodation
    {
        [Key]
        public long AccommodationId { get; set; }
        public string AccommodationName { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Status { get; set; }
        public string IsLive { get; set; }

    }
}
