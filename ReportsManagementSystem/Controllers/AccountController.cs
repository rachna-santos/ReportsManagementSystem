using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReportsManagementSystem.Models;
using System.Threading.Tasks;

namespace ReportsManagementSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly MyDbContext _context;
        public AccountController(MyDbContext context)
        {
            _context = context;
           
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(string P_Password, string P_LoginId)
        {
            int ProcedureType = 101;
            int SessionType = 1;
           

            if (string.IsNullOrEmpty(P_LoginId) && string.IsNullOrEmpty(P_Password))
            {
                ViewBag.twofield = " Email is required";
                ViewBag.twofield2 = " Password is required";
                return View();
            }

            if (string.IsNullOrEmpty(P_LoginId))
            {
                ViewBag.email = "Email is required.";
                return View();
            }

            if (string.IsNullOrEmpty(P_Password))
            {
                ViewBag.password = "Password is required.";
                return View();
            }

            //var userExists = _context.Users.FirstOrDefault(u => u.LoginId == p_LoginId && u.Password== p_Password);

            bool emailInvalid = false;
            bool passwordInvalid = false;
            bool samefield = false;

            // Check if the user exists

            var user = _context.Users.FirstOrDefault(u => u.LoginId == P_LoginId);

            if (user == null)
            {
                ViewBag.EmailError = "Incorrect Email. Please try again.";
                return View();
            }
            else if (user.Password != P_Password)
            {
                // If password does not match
                ViewBag.PasswordError = "Incorrect Password. Please try again.";
                return View();
            }

            HttpContext.Session.SetInt32("UserId", user.UserId);
            _context.Database.ExecuteSqlRaw("execute DBERP.erp.DBERP_SP_DashBorad_Reporting @P_ProcedureType={0},@P_SessionType={1},@P_LoginId={2},@P_Password={3}",
            ProcedureType, SessionType, P_LoginId, P_Password);
            return RedirectToAction("Index", "Home");

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            // jitni bhi session keys set ki thi, sab clear
            HttpContext.Session.Clear();

            // (optional) session cookie bhi force-delete karna ho:
            Response.Cookies.Delete(".AspNetCore.Session");

            return RedirectToAction("Login", "Account");
        }
    }

}

