using System.Diagnostics;
using Cinema_Ticket.DataAccess;
using Cinema_Ticket.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cinema_Ticket.Controllers
{
    public class HomeController : Controller
    {




        public IActionResult Index()
        
        {
            //int countCountCategory = _context.categories.Count();
            //int = _context.categories.Count();
            //int countCountCategory = _context.categories.Count();
            //int countCountCategory = _context.categories.Count();
            


            return View();
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
