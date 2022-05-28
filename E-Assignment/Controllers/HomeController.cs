using E_Assignment.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace E_Assignment.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        [Authorize]
        public IActionResult Index()
        {
            string page = "Index";
            string controller = "Home";
            var role = User.FindFirstValue(ClaimTypes.Role);

            if (role.Equals("Student"))
            {
                page = "ShowDiplomasStudents";
                controller = "Diploma";
            }
            else if (role.Equals("Teacher"))
            {
                page = "ShowDiplomas";
                controller = "Diploma";
            }
            else if (role.Equals("Admin"))
            {
                page = "ListUsers";
                controller = "Administrator";
            }

            return RedirectToAction(page,controller);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
