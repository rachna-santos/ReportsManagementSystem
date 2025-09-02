using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ReportsManagementSystem.Models
{
	public class BookingsData
	{
        public int BookingId { get; set; }
        public long AccommodationId { get; set; }
		public DateTime BookingDate { get; set; }
        public string AccommodationName { get; set; }	
        public DateTime CheckIn { get; set; }
		public DateTime CheckOut { get; set; }
		public int Nights { get; set; }
		public string? RoomName { get; set; }
		public string? RatePlanName { get; set; }
		public string Source { get; set; }
		public string Status { get; set; }
		public decimal Amount { get; set; }
		public string Country { get; set; }
		public DateTime Created { get; set; }
		public string CurrencyName { get; set; }
		public byte BookingStatusId { get; set; }
		public string BookingStatus { get; set; }
		public DateTime CreateDate { get; set; }
		public byte StatusId { get; set; }
        public int TotalBookings { get; set; }
        public int TotalNights { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AverageRate { get; set; }
        public decimal AverageBookingRate { get; set; }

    }
}
