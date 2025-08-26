using System.ComponentModel.DataAnnotations;

namespace ReportsManagementSystem.Models
{
    public class ListGroupandSub
    {
        
        public int ReportGroupId { get; set; }
        public string ReportGroupName { get; set; }
        public int ReportSubGroupId { get; set; }
        public string ReportSubGroupName { get; set; }
    }
}
