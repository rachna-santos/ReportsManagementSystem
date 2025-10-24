using System.ComponentModel.DataAnnotations;

namespace ReportsManagementSystem.Models
{
    public class SummaryReport
    {
        public long AccommodationId { get; set; }
        public string ReportGroupName { get; set; }
        public string ReportSubGroupName { get; set; }
        public string AccommodationName { get; set; }
        public string Source { get; set; }
        public string BookingStatus { get; set; }
        public string BookingCurrency { get; set; }
        public string? ConvertedCurrency { get; set; }
        public int TotalBookings { get; set; }
        public int Nights { get; set; }
        public double ConvertedTotalPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
