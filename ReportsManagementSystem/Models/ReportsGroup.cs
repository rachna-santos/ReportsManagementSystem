using System.ComponentModel.DataAnnotations;

namespace ReportsManagementSystem.Models
{
    public class ReportsGroup
    {
        [Key]
        public int ReportGroupId { get; set; }
        public string ReportGroupName { get; set; }
    }
}
