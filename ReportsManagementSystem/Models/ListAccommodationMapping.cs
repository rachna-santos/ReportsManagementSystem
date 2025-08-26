namespace ReportsManagementSystem.Models
{
    public class ListAccommodationMapping
    {
        //public long AccommodationId { get; set; }
        public string? AccommodationName { get; set; }

        public int ReportGroupId { get; set; }
        public string ReportGroupName { get; set; }
        public int ReportSubGroupId { get; set; }
        public string ReportSubGroupName { get; set; }
        public int IsMapped { get; set; }
    }
}
