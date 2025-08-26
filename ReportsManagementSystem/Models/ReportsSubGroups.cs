using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReportsManagementSystem.Models
{
    public class ReportsSubGroups
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        //public int ReportGroupId { get; set; }
        //public string ReportGroupName { get; set; }
        public int ReportSubGroupId { get; set; }
        public string ReportSubGroupName { get; set; }
    }
}
