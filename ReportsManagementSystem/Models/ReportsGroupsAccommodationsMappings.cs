using System.ComponentModel.DataAnnotations;

namespace ReportsManagementSystem.Models
{
    public class ReportsGroupsAccommodationsMappings
    {
        [Key]
        public int ReportGroupId { get; set; }
        public int ReportSubGroupId { get; set; }
        public long AccommodationId { get; set; }
       
    }
}
