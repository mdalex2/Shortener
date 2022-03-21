using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shortener.Models;
using System.Diagnostics;
using Shortener.DbConection;
using Microsoft.EntityFrameworkCore;

namespace Shortener.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DbConex _db;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, DbConex db)
        {
            _db = db;
            _logger = logger;
        }
        //[Authorize]
        public async Task<IActionResult> Index()
        {
            var url = await _db.UrlConfigs.ToListAsync();
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