using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shortener.Models;
using System.Diagnostics;
using Shortener.DbConection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.WebUtilities;

namespace Shortener.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DbConex _db;
        private readonly IConfiguration _config;

        public HomeController(ILogger<HomeController> logger, DbConex db, IConfiguration config)
        {
            _db = db;
            _logger = logger;
            _config = config;
        }
        
       
        public IActionResult Index()
        {
            return View();
            //return Redirect("https://www.andromaco.com/");
        }

        [HttpGet("{shortCode}")]        
        public async Task<IActionResult> Get(string shortCode)
        {
            string servidorShortener = _config.GetSection("ServidorShortener").Value.ToString();
            string UrlErrorRedirect = _config.GetSection("UrlErrorRedirect").Value.ToString();
            var codigoShort =  await _db.UrlShorts.Where(w => w.UrlCorta == shortCode && (w.FechaExpira == null || w.FechaExpira >= DateTime.Now) ).FirstOrDefaultAsync();
            if (codigoShort != null)
            {
                codigoShort.NumVisitas++;
                await _db.SaveChangesAsync();
            }            
            string urlLarga = codigoShort?.UrlLarga ?? UrlErrorRedirect;
            return Redirect(urlLarga); 
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