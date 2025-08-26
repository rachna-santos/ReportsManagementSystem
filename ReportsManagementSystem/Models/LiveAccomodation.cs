using System.ComponentModel.DataAnnotations;

namespace ReportsManagementSystem.Models
{
    public class LiveAccomodation
    {
        [Key]
        public long AccommodationId { get; set; }
        public string AccommodationName { get; set; }
        public string AccommodationIdName { get; set; }
    }
}
