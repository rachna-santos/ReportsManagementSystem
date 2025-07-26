using Microsoft.EntityFrameworkCore;

namespace ReportsManagementSystem.Models
{
	public class MyDbContext : DbContext
	{
		public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
		{
                
        }
		public DbSet<BookingsData> bookingsDatas { get; set; }
        public DbSet<GetAccommodation> getAccommodations { get; set; }
        public DbSet<GetResources> getResources { get; set; }
        public DbSet<GetBookingStatus> bookingStatuses { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BookingsData>()
            .HasNoKey();
             modelBuilder.Entity<GetAccommodation>()
            .HasNoKey();
            modelBuilder.Entity<GetResources>()
            .HasNoKey();
            modelBuilder.Entity<GetBookingStatus>()
           .HasNoKey();       
        }
    }
   

}
