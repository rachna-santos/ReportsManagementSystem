using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using NuGet.Protocol.Core.Types;
using ReportsManagementSystem.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Text;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ReportsManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly MyDbContext _context;
        public HomeController(ILogger<HomeController> logger, MyDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        public IActionResult Index()
        {
            var user=HttpContext.Session.GetInt32("UserId");
            if (user !=null)
            {
                var getuser = _context.Users.FirstOrDefault(p => p.UserId == user);
                var User = getuser.UserName;
                ViewBag.User = User;
            }

            return View();
        }
        [HttpGet]
        public IActionResult GetAccommodation(string p_AccommodationSearch, int P_Entity = 100, int P_Query = 1)
        {
            var data = _context.getAccommodations
              .FromSqlRaw("EXECUTE SP_ReportsManagement @P_Entity={0},@P_Query={1},@p_AccommodationSearch={2}",
                P_Entity, P_Query, p_AccommodationSearch)
               .ToList();
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(data);
            }

            var Booking = _context.bookingStatuses
                            .FromSqlRaw("EXECUTE SP_ReportsManagement @P_Entity={0},@P_Query={1}",
                                P_Entity, P_Query)
                            .ToList();
            if (!Booking.Any())
            {
                Console.WriteLine("⚠ No data returned from SP_ReportsManagement.");
            }

            ViewBag.Booking = Booking;
            return View(data);
        }
        [HttpGet]
        public IActionResult GetSource(int P_BookingSourceId, int P_Entity = 100, int P_Query = 3)
        {
            var BookingSource = _context.getResources
              .FromSqlRaw("EXECUTE SP_ReportsManagement @P_Entity={0},@P_Query={1},@P_BookingSourceId={2}",
                P_Entity, P_Query, P_BookingSourceId)
               .ToList();
            ViewBag.BookingSource = BookingSource;
            return View();
        }
        [HttpGet]
        public IActionResult BookingData(string p_AccommodationSearch, string P_BookingSource, int? P_BookingStatusId, DateTime? P_StartDate, DateTime? P_EndDate, string P_AccommodationId_CHAR, int searchfilter,int? p_ReportGroupId,int? p_ReportSubGroupId, int P_Entity, int P_Query)
        {

            var user = HttpContext.Session.GetInt32("UserId");

            if (user != null)
            {
                var getuser = _context.Users.FirstOrDefault(p => p.UserId == user);
                var User = getuser.UserName;
                ViewBag.User = User;
            }

            ViewBag.searchfilter = searchfilter;
            ViewBag.P_StartDate = P_StartDate;
            ViewBag.P_EndDate = P_EndDate;
            DateTime? P_BookingStartDate = P_StartDate;
            DateTime? P_BookingEndDate = P_EndDate;
            DateTime? P_CheckInStartDate = P_StartDate;
            DateTime? P_CheckInEndDate = P_EndDate;
            DateTime? P_CheckOutStartDate = P_StartDate;
            DateTime? P_CheckOutEndDate = P_EndDate;
            //setAccommotationName

            var groups = _context.reportsGroups.FromSqlRaw("EXECUTE SP_AccommodationsManagement @P_Entity={0},@P_Query={1}",
              P_Entity = 101, P_Query = 1)
              .ToList();
            ViewBag.groups = groups;

            var subgroups = _context.reportSubGroups.FromSqlRaw("EXECUTE SP_AccommodationsManagement @P_Entity={0},@P_Query={1}",
                      P_Entity = 101, P_Query = 2)
                      .ToList();
            ViewBag.subgroups = subgroups;

            ViewBag.p_ReportGroupId = p_ReportGroupId;
            ViewBag.p_ReportSubGroupId = p_ReportSubGroupId;


            ViewBag.getname = p_AccommodationSearch;
            var data = _context.getAccommodations
             .FromSqlRaw("EXECUTE SP_ReportsManagement @P_Entity={0},@P_Query={1},@p_AccommodationSearch={2}",
               P_Entity = 100, P_Query = 1, p_AccommodationSearch)
              .ToList();
            
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {

                return Json(data);
            }

            //setAccommotationName

            //setbookingStatusesName
            var Booking = _context.bookingStatuses
                         .FromSqlRaw("EXECUTE SP_ReportsManagement @P_Entity={0},@P_Query={1}",
                             P_Entity = 100, P_Query = 2)
                             .ToList();
            if (!Booking.Any())
            {
                Console.WriteLine("⚠ No data returned from SP_ReportsManagement.");
            }

            ViewBag.Booking = Booking;
            ViewBag.selectedBookingStatusId = P_BookingStatusId;

            ViewBag.bookingstatus = (P_BookingStatusId.HasValue && P_BookingStatusId.Value != 0)
                ? Booking.Where(p => p.BookingStatusId == P_BookingStatusId)
                 .Select(x => x.BookingStatus)
                 .FirstOrDefault()
                : "---All P_BookingStatus---";

            //setsourceName
            var BookingSource = _context.getResources
                             .FromSqlRaw("EXECUTE SP_ReportsManagement @P_Entity={0},@P_Query={1}",
                               P_Entity = 100, P_Query = 4)
                              .ToList();
            ViewBag.BookingSource = BookingSource;

            ViewBag.BookingSources = P_BookingSource;

            //setsourceName

            //ShowBookingdata
            // using Microsoft.EntityFrameworkCore;

            if (searchfilter == 1 && P_BookingStartDate.HasValue && P_BookingEndDate.HasValue)
            {
                ViewBag.AccommodationId = P_AccommodationId_CHAR;

                var bookingData = _context.bookingsDatas
                    .FromSqlInterpolated($@"
                 EXECUTE SP_ReportsManagement
                @P_Entity={101},
                @P_Query={1},
                @P_BookingStartDate={P_BookingStartDate},
                @P_BookingEndDate={P_BookingEndDate},
                @P_BookingSource={P_BookingSource},                 -- null OK
                @P_AccommodationId_CHAR={P_AccommodationId_CHAR},   -- null OK
                @P_BookingStatusId={P_BookingStatusId},             -- null/0 OK
                @p_ReportGroupId={p_ReportGroupId},                 -- null/0 OK
                @p_ReportSubGroupId={p_ReportSubGroupId}            -- null/0 OK
        ")
                    .ToList();

                ViewBag.TotalBookings = bookingData.Any() ? bookingData.First().TotalBookings : 0;
                ViewBag.TotalRevenue = bookingData.Any() ? bookingData.First().TotalRevenue : 0;
                ViewBag.totalnight = bookingData.Any() ? bookingData.First().TotalNights : 0;
                ViewBag.AverageBookingRate = bookingData.Any() ? bookingData.First().AverageBookingRate : 0;
                ViewBag.Average = bookingData.Any() ? bookingData.First().AverageRate : 0;
                ViewBag.bookingData = bookingData;
            }
            else if (searchfilter == 2 && P_CheckInStartDate.HasValue && P_CheckInEndDate.HasValue)
            {
                ViewBag.AccommodationId = P_AccommodationId_CHAR;

                var bookingData = _context.bookingsDatas
                    .FromSqlInterpolated($@"
                EXECUTE SP_ReportsManagement
                @P_Entity={101},
                @P_Query={1},
                @P_CheckInStartDate={P_CheckInStartDate},
                @P_CheckInEndDate={P_CheckInEndDate},
                @P_BookingSource={P_BookingSource},
                @P_AccommodationId_CHAR={P_AccommodationId_CHAR},
                @P_BookingStatusId={P_BookingStatusId},
                @p_ReportGroupId={p_ReportGroupId},
                @p_ReportSubGroupId={p_ReportSubGroupId}
        ")
                    .ToList();

                ViewBag.TotalBookings = bookingData.Any() ? bookingData.First().TotalBookings : 0;
                ViewBag.TotalRevenue = bookingData.Any() ? bookingData.First().TotalRevenue : 0;
                ViewBag.totalnight = bookingData.Any() ? bookingData.First().TotalNights : 0;
                ViewBag.AverageBookingRate = bookingData.Any() ? bookingData.First().AverageBookingRate : 0;
                ViewBag.Average = bookingData.Any() ? bookingData.First().AverageRate : 0;
                ViewBag.bookingData = bookingData;
            }
            else if (searchfilter == 3 && P_CheckOutStartDate.HasValue && P_CheckOutEndDate.HasValue)
            {
                ViewBag.AccommodationId = P_AccommodationId_CHAR;

                var bookingData = _context.bookingsDatas
                    .FromSqlInterpolated($@"
            EXECUTE SP_ReportsManagement
                @P_Entity={101},
                @P_Query={1},
                @P_CheckOutStartDate={P_CheckOutStartDate},
                @P_CheckOutEndDate={P_CheckOutEndDate},
                @P_BookingSource={P_BookingSource},
                @P_AccommodationId_CHAR={P_AccommodationId_CHAR},
                @P_BookingStatusId={P_BookingStatusId},
                @p_ReportGroupId={p_ReportGroupId},
                @p_ReportSubGroupId={p_ReportSubGroupId}
        ")
                    .ToList();

                ViewBag.TotalBookings = bookingData.Any() ? bookingData.First().TotalBookings : 0;
                ViewBag.TotalRevenue = bookingData.Any() ? bookingData.First().TotalRevenue : 0;
                ViewBag.totalnight = bookingData.Any() ? bookingData.First().TotalNights : 0;
                ViewBag.AverageBookingRate = bookingData.Any() ? bookingData.First().AverageBookingRate : 0;
                ViewBag.Average = bookingData.Any() ? bookingData.First().AverageRate : 0;
                ViewBag.bookingData = bookingData;
            }

            //if (searchfilter == 1 && ((string.IsNullOrEmpty(P_AccommodationId_CHAR) || !string.IsNullOrEmpty(P_AccommodationId_CHAR)) && P_BookingStartDate.HasValue && P_BookingEndDate.HasValue && (!string.IsNullOrEmpty(P_BookingSource) || P_BookingStatusId > 0) && ((p_ReportGroupId ?? 0) > 0 || (p_ReportSubGroupId ?? 0) > 0)))
            //{
            //    ViewBag.AccommodationId = P_AccommodationId_CHAR;
            //    var bookingData = _context.bookingsDatas.FromSqlRaw("EXECUTE SP_ReportsManagement @P_Entity={0},@P_Query={1},@P_BookingStartDate={2},@P_BookingEndDate={3},@P_BookingSource={4},@P_AccommodationId_CHAR={5},@P_BookingStatusId={6},@p_ReportGroupId={7},@p_ReportSubGroupId={8}",
            //      101, 1, P_BookingStartDate.Value, P_BookingEndDate.Value, P_BookingSource, P_AccommodationId_CHAR, P_BookingStatusId, p_ReportGroupId, p_ReportSubGroupId)
            //   .ToList();

            //    ViewBag.TotalBookings = bookingData.Any() ? bookingData.First().TotalBookings : 0;
            //    ViewBag.TotalRevenue = bookingData.Any() ? bookingData.First().TotalRevenue : 0;
            //    ViewBag.totalnight = bookingData.Any() ? bookingData.First().TotalNights : 0;
            //    ViewBag.AverageBookingRate = bookingData.Any() ? bookingData.First().AverageBookingRate : 0;
            //    ViewBag.Average = bookingData.Any() ? bookingData.First().AverageRate : 0;
            //    ViewBag.bookingData = bookingData;
            //}
            //else if (searchfilter == 2 && ((string.IsNullOrEmpty(P_AccommodationId_CHAR) || !string.IsNullOrEmpty(P_AccommodationId_CHAR)) && P_CheckInStartDate.HasValue && P_CheckInEndDate.HasValue && (!string.IsNullOrEmpty(P_BookingSource) || P_BookingStatusId > 0) && ((p_ReportGroupId ?? 0) > 0 || (p_ReportSubGroupId ?? 0) > 0)))
            //{
            //    ViewBag.AccommodationId = P_AccommodationId_CHAR;
            //    var bookingData = _context.bookingsDatas.FromSqlRaw("EXECUTE SP_ReportsManagement @P_Entity={0},@P_Query={1},@P_CheckInStartDate={2},@P_CheckInEndDate={3},@P_BookingSource={4},@P_AccommodationId_CHAR={5},@P_BookingStatusId={6},@p_ReportGroupId={7},@p_ReportSubGroupId={8}",
            //        101, 1, P_CheckInStartDate.Value, P_CheckInEndDate.Value, P_BookingSource, P_AccommodationId_CHAR, P_BookingStatusId, p_ReportGroupId, p_ReportSubGroupId)
            //   .ToList();
            //    ViewBag.TotalBookings = bookingData.Any() ? bookingData.First().TotalBookings : 0;
            //    ViewBag.TotalRevenue = bookingData.Any() ? bookingData.First().TotalRevenue : 0;
            //    ViewBag.totalnight = bookingData.Any() ? bookingData.First().TotalNights : 0;
            //    ViewBag.AverageBookingRate = bookingData.Any() ? bookingData.First().AverageBookingRate : 0;
            //    ViewBag.Average = bookingData.Any() ? bookingData.First().AverageRate : 0;
            //    ViewBag.bookingData = bookingData;
            //}
            //else if (searchfilter == 3 && ((string.IsNullOrEmpty(P_AccommodationId_CHAR) || !string.IsNullOrEmpty(P_AccommodationId_CHAR)) && P_CheckOutStartDate.HasValue && P_CheckOutEndDate.HasValue && (!string.IsNullOrEmpty(P_BookingSource) || P_BookingStatusId > 0) && ((p_ReportGroupId ?? 0) > 0 || (p_ReportSubGroupId ?? 0) > 0)))
            //{
            //    ViewBag.AccommodationId = P_AccommodationId_CHAR;
            //    var bookingData = _context.bookingsDatas.FromSqlRaw("EXECUTE SP_ReportsManagement @P_Entity={0},@P_Query={1},@P_CheckOutStartDate={2},@P_CheckOutEndDate={3},@P_BookingSource={4},@P_AccommodationId_CHAR={5},@P_BookingStatusId={6},@p_ReportGroupId={7},@p_ReportSubGroupId={8}",
            //        101, 1, P_CheckOutStartDate.Value, P_CheckOutEndDate.Value, P_BookingSource, P_AccommodationId_CHAR, P_BookingStatusId, p_ReportGroupId, p_ReportSubGroupId)
            //   .ToList();
            //    ViewBag.TotalBookings = bookingData.Any() ? bookingData.First().TotalBookings : 0;
            //    ViewBag.TotalRevenue = bookingData.Any() ? bookingData.First().TotalRevenue : 0;
            //    ViewBag.totalnight = bookingData.Any() ? bookingData.First().TotalNights : 0;
            //    ViewBag.AverageBookingRate = bookingData.Any() ? bookingData.First().AverageBookingRate : 0;
            //    ViewBag.Average = bookingData.Any() ? bookingData.First().AverageRate : 0;
            //    ViewBag.bookingData = bookingData;
            //}

            //else if (searchfilter == 1 && !string.IsNullOrEmpty(P_AccommodationId_CHAR) && P_BookingStartDate.HasValue && P_BookingEndDate.HasValue)
            //{
            //    ViewBag.AccommodationId = P_AccommodationId_CHAR;

            //    // BASIC FILTERS BLOCK (Accommodation + Dates only)
            //    var bookingData = _context.bookingsDatas.FromSqlRaw("EXECUTE SP_ReportsManagement @P_Entity={0},@P_Query={1},@P_BookingStartDate={2},@P_BookingEndDate={3},@P_AccommodationId_CHAR={4}",
            //        101, 1, P_BookingStartDate.Value, P_BookingEndDate.Value, P_AccommodationId_CHAR)
            //    .ToList();

            //    ViewBag.TotalBookings = bookingData.Any() ? bookingData.First().TotalBookings : 0;
            //    ViewBag.TotalRevenue = bookingData.Any() ? bookingData.First().TotalRevenue : 0;
            //    ViewBag.totalnight = bookingData.Any() ? bookingData.First().TotalNights : 0;
            //    ViewBag.AverageBookingRate = bookingData.Any() ? bookingData.First().AverageBookingRate : 0;
            //    ViewBag.Average = bookingData.Any() ? bookingData.First().AverageRate : 0;
            //    ViewBag.bookingData = bookingData;
            //}

            //else if (searchfilter == 2 && !string.IsNullOrEmpty(P_AccommodationId_CHAR) && P_CheckInStartDate.HasValue && P_CheckInEndDate.HasValue)
            //{
            //    ViewBag.AccommodationId = P_AccommodationId_CHAR;

            //    var bookingData = _context.bookingsDatas.FromSqlRaw("EXECUTE SP_ReportsManagement @P_Entity={0},@P_Query={1},@P_CheckInStartDate={2},@P_CheckInEndDate={3},@P_AccommodationId_CHAR={4}",
            //     101, 1, P_CheckInStartDate.Value, P_CheckInEndDate.Value, P_AccommodationId_CHAR)
            //   .ToList();
            //    ViewBag.TotalBookings = bookingData.Any() ? bookingData.First().TotalBookings : 0;
            //    ViewBag.TotalRevenue = bookingData.Any() ? bookingData.First().TotalRevenue : 0;
            //    ViewBag.totalnight = bookingData.Any() ? bookingData.First().TotalNights : 0;
            //    ViewBag.AverageBookingRate = bookingData.Any() ? bookingData.First().AverageBookingRate : 0;
            //    ViewBag.Average = bookingData.Any() ? bookingData.First().AverageRate : 0;
            //    ViewBag.bookingData = bookingData;
            //}
            //else if (searchfilter == 3 && !string.IsNullOrEmpty(P_AccommodationId_CHAR) && P_CheckOutStartDate.HasValue && P_CheckOutEndDate.HasValue)
            //{
            //    ViewBag.AccommodationId = P_AccommodationId_CHAR;

            //    var bookingData = _context.bookingsDatas.FromSqlRaw("EXECUTE SP_ReportsManagement @P_Entity={0},@P_Query={1},@P_CheckOutStartDate={2},@P_CheckOutEndDate={3},@P_AccommodationId_CHAR={4}",
            //        101, 1, P_CheckOutStartDate.Value, P_CheckOutEndDate.Value, P_AccommodationId_CHAR)
            //   .ToList();
            //    ViewBag.TotalBookings = bookingData.Any() ? bookingData.First().TotalBookings : 0;
            //    ViewBag.TotalRevenue = bookingData.Any() ? bookingData.First().TotalRevenue : 0;
            //    ViewBag.totalnight = bookingData.Any() ? bookingData.First().TotalNights : 0;
            //    ViewBag.AverageBookingRate = bookingData.Any() ? bookingData.First().AverageBookingRate : 0;
            //    ViewBag.Average = bookingData.Any() ? bookingData.First().AverageRate : 0;
            //    ViewBag.bookingData = bookingData;
            //}
            //ShowBookingdata

            return View();
        }
        [HttpGet]
        public IActionResult AccommodationManagement(string P_GlobalSearch, int? P_IsLive, string P_AccommodationId_CHAR, int P_Entity, int P_Query)
        {
            ViewBag.P_IsLive = P_IsLive;
            ViewBag.getname = P_GlobalSearch;
            var data = _context.liveAccomodations
            .FromSqlRaw("EXECUTE SP_AccommodationsManagement @P_Entity={0},@P_Query={1},@P_GlobalSearch={2}",
               P_Entity = 100, P_Query = 1, P_GlobalSearch)
              .ToList();
            ViewBag.AccommodationId = P_AccommodationId_CHAR;

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(data);
            }

            if (P_AccommodationId_CHAR != null)
            {
                var livedatashow = _context.showLiveAccommodations.FromSqlRaw("EXECUTE SP_AccommodationsManagement @P_Entity={0},@P_Query={1},@P_AccommodationId_CHAR={2},@P_IsLive={3}",
                           100, 2, P_AccommodationId_CHAR, P_IsLive)
                .ToList();
                ViewBag.livedatashow = livedatashow;
            }
            else
            {
                var livedatashow = _context.showLiveAccommodations.FromSqlRaw("EXECUTE SP_AccommodationsManagement @P_Entity={0},@P_Query={1}",
                     100, 2)
                     .ToList();
                ViewBag.livedatashow = livedatashow;
            }
            var user = HttpContext.Session.GetInt32("UserId");
            if (user != null)
            {
                var getuser = _context.Users.FirstOrDefault(p => p.UserId == user);
                var User = getuser.UserName;
                ViewBag.User = User;
            }
            return View();
        }
        [HttpGet]
        public IActionResult Create()
        {
            var user = HttpContext.Session.GetInt32("UserId");
            if (user != null)
            {
                var getuser = _context.Users.FirstOrDefault(p => p.UserId == user);
                var User = getuser.UserName;
                ViewBag.User = User;
            }

            int P_Entity = 101;
            int P_Query = 1;

            var groups = _context.reportsGroups.FromSqlRaw("EXECUTE SP_AccommodationsManagement @P_Entity={0},@P_Query={1}",
                       P_Entity, P_Query)
            .ToList();
            ViewBag.groups = groups;

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ReportsGroups model)
        {
            var user = HttpContext.Session.GetInt32("UserId");
            if (user != null)
            {
                var getuser = _context.Users.FirstOrDefault(p => p.UserId == user);
                var User = getuser.UserName;
                ViewBag.User = User;
            }

            int P_Entity = 101;
            int P_Query = 1;
       
            //var group = _context.ReportsGroups.FirstOrDefault(u => u.ReportGroupId == model.ReportGroupId);

            //if (group.ReportGroupId == model.ReportGroupId)
            //{
            //    ViewBag.EmailError = "Already Exits";
            //    return View();
            //}
          
            var groups = _context.reportsGroups.FromSqlRaw("EXECUTE SP_AccommodationsManagement @P_Entity={0},@P_Query={1}",
                         P_Entity, P_Query)
                       .ToList();
            ViewBag.groups = groups;
            var reportGroupName = model.ReportGroupName;
            ViewBag.reportGroupName = reportGroupName;

            var nextId = _context.ReportsGroups.Max(x => (int?)x.ReportGroupId) ?? 0;
            model.ReportGroupId = nextId + 1;
            _context.ReportsGroups.Add(model);
            _context.SaveChanges();
            return Json(new { success = true, item = new { reportGroupId = model.ReportGroupId, reportGroupName = model.ReportGroupName } });
         
        }
        [HttpGet]
        public IActionResult CreateSubGroup()
        {
            var user = HttpContext.Session.GetInt32("UserId");
            if (user != null)
            {
                var getuser = _context.Users.FirstOrDefault(p => p.UserId == user);
                var User = getuser.UserName;
                ViewBag.User = User;
            }

            int P_Entity = 101;
            int P_Query = 1;

            var subgroups = _context.reportSubGroups.FromSqlRaw("EXECUTE SP_AccommodationsManagement @P_Entity={0},@P_Query={1}",
                      P_Entity = 101, P_Query = 2)
                      .ToList();
            ViewBag.subgroups = subgroups;
            return View();
        }
        [HttpPost]
        public IActionResult CreateSubGroup(ReportsSubGroups model)
        {

            var user = HttpContext.Session.GetInt32("UserId");
            if (user != null)
            {
                var getuser = _context.Users.FirstOrDefault(p => p.UserId == user);
                var User = getuser.UserName;
                ViewBag.User = User;
            }

            int P_Entity = 101;
            int P_Query = 1;

            var subgroups = _context.reportSubGroups.FromSqlRaw("EXECUTE SP_AccommodationsManagement @P_Entity={0},@P_Query={1}",
                      P_Entity = 101, P_Query = 2)
                      .ToList();
            ViewBag.subgroups = subgroups;

            var nextId = _context.ReportsSubGroups.Max(x => (int?)x.ReportSubGroupId) ?? 0;
            model.ReportSubGroupId = nextId + 1;
            _context.ReportsSubGroups.Add(model);
            _context.SaveChanges();
            return Json(new { success = true, item = new { reportSubGroupId = model.ReportSubGroupId, reportSubGroupName = model.ReportSubGroupName } });
        }

        [HttpGet]
        public JsonResult Edit(long p_AccommodationId)
        {


            if (p_AccommodationId != null)
            {
                var acco = _context.Accommodations
                      .FirstOrDefault(p => p.AccommodationId == p_AccommodationId);

                if (acco == null)
                {
                    return Json(null);
                }
                var data = new
                {
                    accommodationId = acco.AccommodationId,
                    isAlive = acco.IsLive
                };
                return Json(data);
            }

            return Json(null);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(long p_AccommodationId, bool P_IsLive)
        {
            int P_Entity = 100;
            int P_Query = 71;
            int P_OwnerId = 1;
            await _context.Database.ExecuteSqlRawAsync("EXECUTE SP_AccommodationsManagement @P_Entity={0},@P_Query={1},@P_OwnerId={2},@p_AccommodationId={3},@P_IsLive={4}",
            P_Entity, P_Query, P_OwnerId, p_AccommodationId, P_IsLive);
            return Json(new
            {
                success = true,
                accommodationId = p_AccommodationId,
                isLive = P_IsLive
            });
        }
        [HttpGet]
        public IActionResult GroupsReport()
        {
            var user = HttpContext.Session.GetInt32("UserId");
            if (user != null)
            {
                var getuser = _context.Users.FirstOrDefault(p => p.UserId == user);
                var User = getuser.UserName;
                ViewBag.User = User;
            }
            int P_Entity = 101;
            int P_Query = 1;
           
            var groups = _context.reportsGroups.FromSqlRaw("EXECUTE SP_AccommodationsManagement @P_Entity={0},@P_Query={1}",
                       P_Entity, P_Query)
                       .ToList();
            ViewBag.groups = groups;

            var subgroups = _context.reportSubGroups.FromSqlRaw("EXECUTE SP_AccommodationsManagement @P_Entity={0},@P_Query={1}",
                      P_Entity = 101, P_Query = 2)
                      .ToList();
            ViewBag.subgroups = subgroups;

            var showgroup = _context.listGroupandSubs.FromSqlRaw("EXECUTE SP_AccommodationsManagement @P_Entity={0},@P_Query={1}",
                    P_Entity = 101, P_Query = 3)
                    .ToList();
            //ViewBag.showgroup = showgroup;
            return View(showgroup);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GroupsReport(int p_ReportGroupId, int p_ReportSubGroupId)
        {
            var user = HttpContext.Session.GetInt32("UserId");
            if (user != null)
            {
                var getuser = _context.Users.FirstOrDefault(p => p.UserId == user);
                var User = getuser.UserName;
                ViewBag.User = User;
            }
            int P_Entity = 101;
            int P_Query = 1;
            var groups = _context.reportsGroups.FromSqlRaw("EXECUTE SP_AccommodationsManagement @P_Entity={0},@P_Query={1}",
                       P_Entity, P_Query)
                       .ToList();
            ViewBag.groups = groups;

            var subgroups = _context.reportSubGroups.FromSqlRaw("EXECUTE SP_AccommodationsManagement @P_Entity={0},@P_Query={1}",
                      P_Entity = 101, P_Query = 2)
                      .ToList();
            ViewBag.subgroups = subgroups;

            var showgroup = _context.listGroupandSubs.FromSqlRaw("EXECUTE SP_AccommodationsManagement @P_Entity={0},@P_Query={1}",
                    P_Entity = 101, P_Query = 3)
                    .ToList();
            ViewBag.showgroup = showgroup;
         

            ViewBag.p_ReportGroupId = p_ReportGroupId;
            ViewBag.p_ReportSubGroupId = p_ReportSubGroupId;

            await _context.Database.
            ExecuteSqlRawAsync("EXECUTE SP_AccommodationsManagement @P_Entity={0},@P_Query={1},@p_ReportGroupId={2},@p_ReportSubGroupId={3}",
               P_Entity = 101, P_Query = 51, p_ReportGroupId, p_ReportSubGroupId);

            return Ok(new { success = true, message = "Data successfully inserted" });

        }
        [HttpGet]
        public IActionResult SelectGroup(int p_ReportGroupId)
        {
            const int P_Entity = 101;

            // 1) MASTER list of ALL subgroups (mapped + unmapped)
            const int P_Query_AllSubs = 2;
            var allSubs = _context.reportSubGroups
                .FromSqlRaw("EXECUTE SP_AccommodationsManagement @P_Entity={0}, @P_Query={1}",
                    P_Entity, P_Query_AllSubs)
                .AsNoTracking()
                .AsEnumerable() // ⬅️ switch to client-side BEFORE Select/GroupBy
                .Select(r => new { r.ReportSubGroupId, r.ReportSubGroupName })
                .GroupBy(x => new { x.ReportSubGroupId, x.ReportSubGroupName })
                .Select(g => g.Key)
                .ToList();

            // 2) MAPPINGS (Group <-> SubGroup) — only to know what's already assigned to THIS group
            const int P_Query_Mappings = 3; // SP: returns mappings (ReportGroupId, ReportSubGroupId, ...)
            var mappings = _context.listGroupandSubs
                .FromSqlRaw("EXECUTE SP_AccommodationsManagement @P_Entity={0}, @P_Query={1}",
                    P_Entity, P_Query_Mappings)
                .AsNoTracking()
                .ToList();

            var assignedToThisGroup = mappings
                .Where(m => m.ReportGroupId == p_ReportGroupId)
                .Select(m => m.ReportSubGroupId)
                .Distinct()
                .ToHashSet();

            // 3) Show ALL subgroups EXCEPT the ones already assigned to THIS group
            var options = allSubs
                .Where(s => !assignedToThisGroup.Contains(s.ReportSubGroupId))
                .OrderBy(s => s.ReportSubGroupName)
                .ToList();

            return Json(options);
        }

        //public IActionResult SelectGroup(int p_ReportGroupId)
        //{
        //    const int P_Entity = 101;
        //    const int P_Query = 3;

        //    var rows = _context.listGroupandSubs
        //        .FromSqlRaw(
        //            "EXECUTE SP_AccommodationsManagement @P_Entity={0}, @P_Query={1}",
        //            P_Entity, P_Query)
        //        .AsNoTracking()
        //        .ToList();

        //    var allSubs = rows
        //        .Select(r => new { r.ReportSubGroupId, r.ReportSubGroupName })
        //        .Distinct()
        //        .ToList();

        //    // SubGroups already assigned to the SELECTED group
        //    var assignedToSelected = rows
        //        .Where(r => r.ReportGroupId == p_ReportGroupId)
        //        .Select(r => r.ReportSubGroupId)
        //        .Distinct()
        //        .ToHashSet();

        //    // Show subgroups NOT yet mapped to the selected group
        //    var options = allSubs
        //        .Where(s => !assignedToSelected.Contains(s.ReportSubGroupId))
        //        .OrderBy(s => s.ReportSubGroupName)
        //        .ToList();
        //    return Json(options);
        //}

        [HttpGet]
        public IActionResult CreateReportsGroupsAccommodations(string p_AccommodationSearch, long? P_AccommodationId,int? p_ReportGroupId,int? p_ReportSubGroupId)
        {
            var user = HttpContext.Session.GetInt32("UserId");
            if (user != null)
            {
                var getuser = _context.Users.FirstOrDefault(p => p.UserId == user);
                var User = getuser.UserName;
                ViewBag.User = User;
            }

            int P_Entity = 101;
            int P_Query = 4;

            ViewBag.getname = p_AccommodationSearch;
            ViewBag.p_AccommodationId = P_AccommodationId;

            var data = _context.getAccommodations
            .FromSqlRaw("EXECUTE SP_ReportsManagement @P_Entity={0},@P_Query={1},@p_AccommodationSearch={2}",
              P_Entity = 100, P_Query = 1, p_AccommodationSearch)
             .ToList();

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(data);
            }

         
            var groups = _context.reportsGroups.FromSqlRaw("EXECUTE SP_AccommodationsManagement @P_Entity={0},@P_Query={1}",
                P_Entity = 101, P_Query = 1)
                .ToList();
            ViewBag.groups = groups;

            var subgroups = _context.reportSubGroups.FromSqlRaw("EXECUTE SP_AccommodationsManagement @P_Entity={0},@P_Query={1}",
                      P_Entity = 101, P_Query = 2)
                      .ToList();
            ViewBag.subgroups = subgroups;

            ViewBag.p_ReportGroupId = p_ReportGroupId;
            ViewBag.p_ReportSubGroupId = p_ReportSubGroupId;
           

            if (P_AccommodationId > 0 && p_ReportGroupId.HasValue && p_ReportSubGroupId.HasValue)
            {
                var showmapping = _context.getMappings.FromSqlRaw("EXECUTE SP_AccommodationsManagement @P_Entity={0},@P_Query={1},@p_ReportGroupId={2},@p_ReportSubGroupId={3},@P_AccommodationId={4}",
              P_Entity = 101, P_Query = 4, p_ReportGroupId.Value, p_ReportSubGroupId.Value, P_AccommodationId)
              .ToList();
                ViewBag.showmapping = showmapping;
               

            }
            else if(P_AccommodationId > 0)
            {
                var showmapping = _context.getMappings.FromSqlRaw("EXECUTE SP_AccommodationsManagement @P_Entity={0},@P_Query={1},@P_AccommodationId={2}",
                P_Entity = 101, P_Query = 4, P_AccommodationId)
                .ToList();
                ViewBag.showmapping = showmapping;
            }
            else if (p_ReportGroupId!=null)
            {
                var showmapping = _context.getMappings.FromSqlRaw("EXECUTE SP_AccommodationsManagement @P_Entity={0},@P_Query={1},@p_ReportGroupId={2}",
                P_Entity = 101, P_Query = 4, p_ReportGroupId)
                .ToList();
                ViewBag.showmapping = showmapping;
            }

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateReportsGroupsAccommodations(long P_AccommodationId,string p_AccommodationSearch, int? p_ReportGroupId, int? p_ReportSubGroupId)
        {
            var user = HttpContext.Session.GetInt32("UserId");
            if (user != null)
            {
                var getuser = _context.Users.FirstOrDefault(p => p.UserId == user);
                var User = getuser.UserName;
                ViewBag.User = User;
            }

            try
            {
                int P_Entity = 101;
                int P_Query = 4;
                // yahi pe mapping insert chalani hai
                await _context.Database.ExecuteSqlRawAsync(
                    "EXECUTE SP_AccommodationsManagement @P_Entity={0},@P_Query={1},@P_ReportGroupId={2},@P_ReportSubGroupId={3},@P_AccommodationId={4}",
                    101, 52, p_ReportGroupId, p_ReportSubGroupId, P_AccommodationId);

                // agar AJAX hai to JSON me success flag bhejo
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return Json(new { success = true, message = "Mapping successfully saved." });

                TempData["Message"] = "Mapping successfully saved.";
                return RedirectToAction(nameof(CreateReportsGroupsAccommodations),
                    new { p_AccommodationSearch, P_AccommodationId });


                ViewBag.getname = p_AccommodationSearch;
                ViewBag.p_AccommodationId = P_AccommodationId;
                ViewBag.p_ReportGroupId = p_ReportGroupId;
                ViewBag.p_ReportSubGroupId = p_ReportSubGroupId;
                var data = _context.getAccommodations

                .FromSqlRaw("EXECUTE SP_ReportsManagement @P_Entity={0},@P_Query={1},@p_AccommodationSearch={2}",
                  P_Entity = 100, P_Query = 1, p_AccommodationSearch)
                 .ToList();

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(data);
                }

                if (P_AccommodationId > 0 && p_ReportGroupId.HasValue && p_ReportSubGroupId.HasValue)
                {
                    var showmapping = _context.getMappings.FromSqlRaw("EXECUTE SP_AccommodationsManagement @P_Entity={0},@P_Query={1},@p_ReportGroupId={2},@p_ReportSubGroupId={3},@P_AccommodationId={4}",
                  P_Entity = 101, P_Query = 4, p_ReportGroupId.Value, p_ReportSubGroupId.Value, P_AccommodationId)
                  .ToList();
                    ViewBag.showmapping = showmapping;
                }

                else if (P_AccommodationId > 0)
                {
                    var showmapping = _context.getMappings.FromSqlRaw("EXECUTE SP_AccommodationsManagement @P_Entity={0},@P_Query={1},@P_AccommodationId={2}",
                    P_Entity = 101, P_Query = 4, P_AccommodationId)
                    .ToList();
                    ViewBag.showmapping = showmapping;
                }
                else if (p_ReportGroupId != null)
                {
                    var showmapping = _context.getMappings.FromSqlRaw("EXECUTE SP_AccommodationsManagement @P_Entity={0},@P_Query={1},@p_ReportGroupId={2}",
                    P_Entity = 101, P_Query = 4, p_ReportGroupId)
                    .ToList();
                    ViewBag.showmapping = showmapping;
                }

            }
            catch (Exception ex)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return StatusCode(500, new { success = false, message = ex.Message });

                ModelState.AddModelError("", ex.Message);

                return View();
            }
           
        }
        [HttpGet]
        public IActionResult ListAccommodationMapping(int? p_ReportGroupId, int? p_ReportSubGroupId)
        {
            var user = HttpContext.Session.GetInt32("UserId");
            if (user != null)
            {
                var getuser = _context.Users.FirstOrDefault(p => p.UserId == user);
                var User = getuser.UserName;
                ViewBag.User = User;
            }

            int P_Entity = 101;
            int P_Query = 4;

            if (p_ReportGroupId!=null)
            {
                var listmapping = _context.listAccommodationMappings
              .FromSqlRaw("EXEC SP_AccommodationsManagement @P_Entity={0},@P_Query={1},@p_ReportGroupId={2},@p_ReportSubGroupId={3}",
                          P_Entity, P_Query, p_ReportGroupId, p_ReportSubGroupId).ToList();

                ViewBag.listmapping = listmapping;
            }
            //else{

            //    var listmapping = _context.listAccommodationMappings
            //    .FromSqlRaw("EXEC SP_AccommodationsManagement @P_Entity={0},@P_Query={1}",
            //             P_Entity, P_Query).ToList();

            //    ViewBag.listmapping = listmapping;
            //}

            var groups = _context.reportsGroups.FromSqlRaw("EXECUTE SP_AccommodationsManagement @P_Entity={0},@P_Query={1}",
                P_Entity = 101, P_Query = 1)
                .ToList();
            ViewBag.groups = groups;

            var subgroups = _context.reportSubGroups.FromSqlRaw("EXECUTE SP_AccommodationsManagement @P_Entity={0},@P_Query={1}",
                      P_Entity = 101, P_Query = 2)
                      .ToList();
            ViewBag.subgroups = subgroups;

            ViewBag.p_ReportGroupId = p_ReportGroupId;
            ViewBag.p_ReportSubGroupId = p_ReportSubGroupId;

            return View();
        }
        [HttpGet]
        public IActionResult GetGroupsByAccommodation(long P_AccommodationId)
        {
            int P_Entity = 101;
            int P_Query = 4;
            var groups = _context.groupSubGroupCombs
                .FromSqlRaw("EXEC SP_AccommodationsManagement @P_Entity={0},@P_Query={1},@P_AccommodationId={2}",
                            P_Entity, P_Query, P_AccommodationId).ToList();
            return Json(groups);
        }

    }
}
