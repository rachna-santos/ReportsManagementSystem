using LogManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace ReportsManagementSystem.Models
{
	public class MyDbContext : DbContext
	{
		public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
		{
                
        }

        public DbSet<Users> Users { get; set; }
        public DbSet<BookingsData> bookingsDatas { get; set; }
        public DbSet<GetAccommodation> getAccommodations { get; set; }
        public DbSet<GetResources> getResources { get; set; }
        public DbSet<GetBookingStatus> bookingStatuses { get; set; }
        public DbSet<ShowLiveAccommodation> showLiveAccommodations { get; set; }
        public DbSet<LiveAccomodation> liveAccomodations { get; set; }
        public DbSet<Accommodations> Accommodations { get; set; }
        public DbSet<ReportsGroup> reportsGroups { get; set; }
        public DbSet<ReportSubGroup> reportSubGroups { get; set; }
        public DbSet<ListGroupandSub> listGroupandSubs { get; set; }
        public DbSet<ReportsSubGroups> ReportsSubGroups { get; set; }
        public DbSet<ReportsGroupsMappings> ReportsGroupsMappings { get; set; }
        public DbSet<GroupSubGroupComb> groupSubGroupCombs { get; set; }
        public DbSet<ReportsGroupsAccommodationsMappings> ReportsGroupsAccommodationsMappings { get; set; }
        public DbSet<getMapping> getMappings { get; set; }
        public DbSet<ListAccommodationMapping> listAccommodationMappings { get; set; }
        public DbSet<ReportsGroups> ReportsGroups { get; set; }
        public DbSet<SummaryReport> summaryReports { get; set; }
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
            modelBuilder.Entity<LiveAccomodation>()
           .HasNoKey();
            modelBuilder.Entity<ListGroupandSub>()
          .HasNoKey();
            modelBuilder.Entity<getMapping>()
             .HasNoKey();
            modelBuilder.Entity<GroupSubGroupComb>()
          .HasNoKey();
            modelBuilder.Entity<ListAccommodationMapping>()
            .HasNoKey();

            modelBuilder.Entity<SummaryReport>()
             .HasNoKey();


            modelBuilder.Entity<ReportsGroups>(e =>
            {
                e.HasKey(x => x.ReportGroupId);
                e.Property(x => x.ReportGroupId).ValueGeneratedOnAdd(); // or .UseIdentityColumn();
            });

            modelBuilder.Entity<ReportsSubGroups>(e =>
            {
                e.HasKey(x => x.ReportSubGroupId);
                e.Property(x => x.ReportSubGroupId).ValueGeneratedOnAdd(); // or .UseIdentityColumn();
            });


        }
    }
}



