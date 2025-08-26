using System.ComponentModel.DataAnnotations;

namespace ReportsManagementSystem.Models
{
    public class ReportSubGroup
    {
        [Key]
        public int ReportSubGroupId { get; set; }
        public string ReportSubGroupName { get; set; }
    }
}

