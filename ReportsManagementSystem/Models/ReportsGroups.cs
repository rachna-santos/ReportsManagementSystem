using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReportsManagementSystem.Models
{
    public class ReportsGroups
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ReportGroupId { get; set; }

        [Required]
        public string ReportGroupName { get; set; }
    }
}
