using System.ComponentModel.DataAnnotations;

namespace ReportsManagementSystem.Models
{
    public class ReportsGroupsMappings
    {
        [Key]
        public int ReportGroupId { get; set; }
        public int ReportSubGroupId { get; set; }
    }
}
