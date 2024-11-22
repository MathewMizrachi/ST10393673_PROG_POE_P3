using Microsoft.AspNetCore.Mvc;
using ST10393673_PROG6212_POE.Models;
using System.Diagnostics;

namespace ST10393673_PROG6212_POE.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        // Constructor to inject the logger service
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        // Action for the Home page (Index view)
        public IActionResult Index()
        {
            // You can include any logic here, such as retrieving data for the view
            return View();
        }

        // Action for the Privacy page
        public IActionResult Privacy()
        {
            // Logic for Privacy page if any
            return View();
        }

        // Error handling action, returns the Error view with an ErrorViewModel
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            // Capture the request ID for the error page, either from Activity or HTTP trace
            var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            return View(new ErrorViewModel { RequestId = requestId });
        }
    }
}
