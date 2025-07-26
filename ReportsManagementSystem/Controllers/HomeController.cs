using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using NuGet.Protocol.Core.Types;
using ReportsManagementSystem.Models;
using System.Diagnostics;
using System.Drawing.Printing;
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
        public IActionResult BookingData(string p_AccommodationSearch, string P_BookingSource, int? P_BookingStatusId,DateTime? P_StartDate, DateTime? P_EndDate, long P_AccommodationId, int searchfilter, int P_Entity, int P_Query)
        {
            ViewBag.searchfilter = searchfilter;
            ViewBag.P_StartDate = P_StartDate;
            ViewBag.P_EndDate = P_EndDate;
            DateTime? P_BookingStartDate = P_StartDate;
            DateTime? P_BookingEndDate = P_EndDate;
            DateTime? P_CheckInStartDate = P_StartDate;
            DateTime? P_CheckInEndDate = P_EndDate;
            DateTime? P_CheckOutStartDate = P_StartDate;
            DateTime? P_CheckOutEndDate= P_EndDate;

            //setAccommotationName
            ViewBag.getname = p_AccommodationSearch;
            var data = _context.getAccommodations
             .FromSqlRaw("EXECUTE SP_ReportsManagement @P_Entity={0},@P_Query={1},@p_AccommodationSearch={2}",
               P_Entity = 100, P_Query = 1, p_AccommodationSearch)
              .ToList();
                ViewBag.AccommodationId = P_AccommodationId;
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

            //ViewBag.Booking = Booking;
            //ViewBag.selectedBookingStatusId = P_BookingStatusId;
            //ViewBag.bookingstatus = Booking
            //.Where(p => p.BookingStatusId == P_BookingStatusId)
            //.Select(x => x.BookingStatus)
            //.FirstOrDefault();

            //setbookingStatusesName//

            //setsourceName
            var BookingSource = _context.getResources
                             .FromSqlRaw("EXECUTE SP_ReportsManagement @P_Entity={0},@P_Query={1}",
                               P_Entity = 100, P_Query = 4)
                              .ToList();
                 ViewBag.BookingSource = BookingSource;

                 ViewBag.BookingSources = P_BookingSource;

            //setsourceName

            //ShowBookingdata
            if (searchfilter == 1 && P_AccommodationId > 0 && P_BookingStartDate.HasValue && P_BookingEndDate.HasValue && !string.IsNullOrEmpty(P_BookingSource) && P_BookingStatusId > 0)
                {
                    var bookingData = _context.bookingsDatas.FromSqlRaw("EXECUTE SP_ReportsManagement @P_Entity={0},@P_Query={1},@P_BookingStartDate={2},@P_BookingEndDate={3},@P_BookingSource={4},@P_AccommodationId={5},@P_BookingStatusId={6}",
                        101, 1, P_BookingStartDate.Value, P_BookingEndDate.Value, P_BookingSource, P_AccommodationId, P_BookingStatusId)
                   .ToList();
                    ViewBag.TotalBookings = bookingData.Any() ? bookingData.First().TotalBookings : 0;
                    ViewBag.TotalRevenue = bookingData.Any() ? bookingData.First().TotalRevenue : 0;
                    ViewBag.totalnight = bookingData.Any() ? bookingData.First().TotalNights : 0;
                    ViewBag.AverageBookingRate = bookingData.Any() ? bookingData.First().AverageBookingRate : 0;
                    ViewBag.Average = bookingData.Any() ? bookingData.First().AverageRate : 0;
                    ViewBag.bookingData = bookingData;
                }
                else if(searchfilter==2 && P_AccommodationId > 0 && P_CheckInStartDate.HasValue && P_CheckInEndDate.HasValue && !string.IsNullOrEmpty(P_BookingSource) && P_BookingStatusId > 0)
                {
                    var bookingData = _context.bookingsDatas.FromSqlRaw("EXECUTE SP_ReportsManagement @P_Entity={0},@P_Query={1},@P_CheckInStartDate={2},@P_CheckInEndDate={3},@P_BookingSource={4},@P_AccommodationId={5},@P_BookingStatusId={6}",
                        101, 1, P_CheckInStartDate.Value, P_CheckInEndDate.Value, P_BookingSource, P_AccommodationId, P_BookingStatusId)
                   .ToList();
                    ViewBag.TotalBookings = bookingData.Any() ? bookingData.First().TotalBookings : 0;
                    ViewBag.TotalRevenue = bookingData.Any() ? bookingData.First().TotalRevenue : 0;
                    ViewBag.totalnight = bookingData.Any() ? bookingData.First().TotalNights : 0;
                    ViewBag.AverageBookingRate = bookingData.Any() ? bookingData.First().AverageBookingRate : 0;
                    ViewBag.Average = bookingData.Any() ? bookingData.First().AverageRate : 0;
                    ViewBag.bookingData = bookingData;
                }
                else if (searchfilter == 3 && P_AccommodationId > 0 && P_CheckOutStartDate.HasValue && P_CheckOutEndDate.HasValue && !string.IsNullOrEmpty(P_BookingSource) && P_BookingStatusId > 0)
                {
                    var bookingData = _context.bookingsDatas.FromSqlRaw("EXECUTE SP_ReportsManagement @P_Entity={0},@P_Query={1},@P_CheckOutStartDate={2},@P_CheckOutEndDate={3},@P_BookingSource={4},@P_AccommodationId={5},@P_BookingStatusId={6}",
                        101, 1, P_CheckOutStartDate.Value, P_CheckOutEndDate.Value, P_BookingSource, P_AccommodationId, P_BookingStatusId)
                   .ToList();
                    ViewBag.TotalBookings = bookingData.Any() ? bookingData.First().TotalBookings : 0;
                    ViewBag.TotalRevenue = bookingData.Any() ? bookingData.First().TotalRevenue : 0;
                    ViewBag.totalnight = bookingData.Any() ? bookingData.First().TotalNights : 0;
                    ViewBag.AverageBookingRate = bookingData.Any() ? bookingData.First().AverageBookingRate : 0;
                    ViewBag.Average = bookingData.Any() ? bookingData.First().AverageRate : 0;
                    ViewBag.bookingData = bookingData;
                }

                else if (searchfilter == 1 && P_AccommodationId > 0 && P_BookingStartDate.HasValue && P_BookingEndDate.HasValue)
                {
                    // BASIC FILTERS BLOCK (Accommodation + Dates only)
                    var bookingData = _context.bookingsDatas.FromSqlRaw("EXECUTE SP_ReportsManagement @P_Entity={0},@P_Query={1},@P_BookingStartDate={2},@P_BookingEndDate={3},@P_AccommodationId={4}",
                        101, 1, P_BookingStartDate.Value, P_BookingEndDate.Value, P_AccommodationId)
                        .ToList();
                    ViewBag.TotalBookings = bookingData.Any() ? bookingData.First().TotalBookings : 0;
                    ViewBag.TotalRevenue = bookingData.Any() ? bookingData.First().TotalRevenue : 0;
                    ViewBag.totalnight = bookingData.Any() ? bookingData.First().TotalNights : 0;
                    ViewBag.AverageBookingRate = bookingData.Any() ? bookingData.First().AverageBookingRate : 0;
                    ViewBag.Average = bookingData.Any() ? bookingData.First().AverageRate : 0;
                    ViewBag.bookingData = bookingData;
                }

                else if (searchfilter == 2 && P_AccommodationId > 0 && P_CheckInStartDate.HasValue && P_CheckInEndDate.HasValue)
                {
                    var bookingData = _context.bookingsDatas.FromSqlRaw("EXECUTE SP_ReportsManagement @P_Entity={0},@P_Query={1},@P_CheckInStartDate={2},@P_CheckInEndDate={3},@P_AccommodationId={4}",
                        101, 1, P_CheckInStartDate.Value, P_CheckInEndDate.Value,P_AccommodationId)
                   .ToList();
                    ViewBag.TotalBookings = bookingData.Any() ? bookingData.First().TotalBookings : 0;
                    ViewBag.TotalRevenue = bookingData.Any() ? bookingData.First().TotalRevenue : 0;
                    ViewBag.totalnight = bookingData.Any() ? bookingData.First().TotalNights : 0;
                    ViewBag.AverageBookingRate = bookingData.Any() ? bookingData.First().AverageBookingRate : 0;
                    ViewBag.Average = bookingData.Any() ? bookingData.First().AverageRate : 0;
                    ViewBag.bookingData = bookingData;
                }
                else if (searchfilter == 3 && P_AccommodationId > 0 && P_CheckOutStartDate.HasValue && P_CheckOutEndDate.HasValue)
                {
                    var bookingData = _context.bookingsDatas.FromSqlRaw("EXECUTE SP_ReportsManagement @P_Entity={0},@P_Query={1},@P_CheckOutStartDate={2},@P_CheckOutEndDate={3},@P_AccommodationId={4}",
                        101, 1, P_CheckOutStartDate.Value, P_CheckOutEndDate.Value, P_AccommodationId)
                   .ToList();
                    ViewBag.TotalBookings = bookingData.Any() ? bookingData.First().TotalBookings : 0;
                    ViewBag.TotalRevenue = bookingData.Any() ? bookingData.First().TotalRevenue : 0;
                    ViewBag.totalnight = bookingData.Any() ? bookingData.First().TotalNights : 0;
                    ViewBag.AverageBookingRate = bookingData.Any() ? bookingData.First().AverageBookingRate : 0;
                    ViewBag.Average = bookingData.Any() ? bookingData.First().AverageRate : 0;
                    ViewBag.bookingData = bookingData;

                }
            //ShowBookingdata
            return View();
        }

        //      public IActionResult BookingData(string p_AccommodationSearch, string P_BookingSource, int P_BookingStatusId, DateTime? P_BookingStartDate, DateTime? P_BookingEndDate, long P_AccommodationId, int searchfilter , int P_Entity, int P_Query)
        //      {
        //          ViewBag.getname = p_AccommodationSearch;
        //          var data = _context.getAccommodations
        //           .FromSqlRaw("EXECUTE SP_ReportsManagement @P_Entity={0},@P_Query={1},@p_AccommodationSearch={2}",
        //             P_Entity = 100, P_Query = 1, p_AccommodationSearch)
        //            .ToList();
        //              ViewBag.AccommodationId = P_AccommodationId;
        //              if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        //              {

        //                  return Json(data);

        //              }

        //              var Booking = _context.bookingStatuses
        //                           .FromSqlRaw("EXECUTE SP_ReportsManagement @P_Entity={0},@P_Query={1}",
        //                               P_Entity = 100, P_Query = 2)
        //                           .ToList();
        //              if (!Booking.Any())
        //              {
        //                  Console.WriteLine("⚠ No data returned from SP_ReportsManagement.");
        //              }

        //              ViewBag.Booking = Booking;

        //                  var BookingSource = _context.getResources
        //                               .FromSqlRaw("EXECUTE SP_ReportsManagement @P_Entity={0},@P_Query={1}",
        //                                 P_Entity = 100, P_Query = 4)
        //                                .ToList();
        //                  ViewBag.BookingSource = BookingSource;

        //          // Check if all filters are selected (Source + StatusId)
        //          if (P_AccommodationId > 0 && P_BookingStartDate.HasValue && P_BookingEndDate.HasValue && !string.IsNullOrEmpty(P_BookingSource) && P_BookingStatusId > 0)
        //          {

        //              // FULL FILTERS BLOCK
        //              var bookingData = _context.bookingsDatas.FromSqlRaw("EXECUTE SP_ReportsManagement @P_Entity={0},@P_Query={1},@P_BookingStartDate={2},@P_BookingEndDate={3},@P_BookingSource={4},@P_AccommodationId={5},@P_BookingStatusId={6}",
        //                  101, 1, P_BookingStartDate.Value, P_BookingEndDate.Value, P_BookingSource, P_AccommodationId, P_BookingStatusId)
        //                  .ToList();

        //              ViewBag.TotalBookings = bookingData.Any() ? bookingData.First().TotalBookings : 0;
        //              ViewBag.TotalRevenue = bookingData.Any() ? bookingData.First().TotalRevenue : 0;
        //              ViewBag.totalnight = bookingData.Any() ? bookingData.First().TotalNights : 0;
        //              ViewBag.AverageBookingRate = bookingData.Any() ? bookingData.First().AverageBookingRate : 0;
        //              ViewBag.Average = bookingData.Any() ? bookingData.First().AverageRate : 0;
        //              ViewBag.bookingData = bookingData;
        //          }

        //          else if (P_AccommodationId > 0 && P_BookingStartDate.HasValue && P_BookingEndDate.HasValue)
        //          {
        //              // BASIC FILTERS BLOCK (Accommodation + Dates only)
        //              var bookingData = _context.bookingsDatas.FromSqlRaw("EXECUTE SP_ReportsManagement @P_Entity={0},@P_Query={1},@P_BookingStartDate={2},@P_BookingEndDate={3},@P_AccommodationId={4}",
        //                  101, 1, P_BookingStartDate.Value, P_BookingEndDate.Value, P_AccommodationId)
        //                  .ToList();
        //              ViewBag.TotalBookings = bookingData.Any() ? bookingData.First().TotalBookings : 0;
        //              ViewBag.TotalRevenue = bookingData.Any() ? bookingData.First().TotalRevenue : 0;
        //              ViewBag.totalnight = bookingData.Any() ? bookingData.First().TotalNights : 0;
        //              ViewBag.AverageBookingRate = bookingData.Any() ? bookingData.First().AverageBookingRate : 0;
        //              ViewBag.Average = bookingData.Any() ? bookingData.First().AverageRate : 0;
        //              ViewBag.bookingData = bookingData;


        //          }
        //          return View();
        //}


        //int pageSize = 10;
        //int currentPage = page ?? 1;
        //int totalItems = bookingData.Count;

        //var pagedItems = bookingData
        //    .OrderBy(x => x.AccommodationId)
        //    .Skip((currentPage - 1) * pageSize)
        //    .Take(pageSize)
        //    .ToList();

        //ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);
        //ViewBag.CurrentPage = currentPage;

        //return View(pagedItems); // ✅ Make sure this is pagedItems, NOT bookingData
   
    }
}