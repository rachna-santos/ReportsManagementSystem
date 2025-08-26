using System.ComponentModel.DataAnnotations;

namespace ReportsManagementSystem.Models
{
    public class Accommodations
    {
        [Key]
        public long AccommodationId { get; set; }
        public bool IsLive { get; set; }
        public int OwnerId { get; set; }

    }
}

