using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shortener.DbConection;
using Shortener.Models;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Linq;

namespace Shortener.Controllers
{
    public class AcortadorController : Controller
    {
        private readonly DbConex _db;
        private readonly IConfiguration _configuration;
        private static Random random = new Random();

        public AcortadorController(DbConex db, IConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
        }
        public static string ObtenerCodigoAleatorio(int length)
        {
            const string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(characters, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public async Task <IActionResult> Index()
        {
            IEnumerable<UrlConfigs> lstUrl = await _db.UrlConfigs.OrderByDescending(o => o.FechaCreacion).Take(20).ToListAsync();
            return View(lstUrl);
        }
        public async Task<IActionResult> Upsert(int? id)
        {
            UrlConfigs? url = new UrlConfigs();
            if (id == null)
            {
                return View(url);
            }
            url = await _db.UrlConfigs.FindAsync(id.GetValueOrDefault());
            if (url == null)
            {
                return NotFound();
            }            
            return PartialView(url);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(UrlConfigs url)
        {            
            if (ModelState.IsValid)
            {
                UrlConfigs? urlDb = null;
                await _db.Database.BeginTransactionAsync();
                if (url.ID == 0)
                {
                    url.FechaCreacion = DateTime.Now;
                    url.FechaModificacion = DateTime.Now;                    
                    await _db.UrlConfigs.AddAsync(url);                    
                }
                else
                {
                    urlDb = await _db.UrlConfigs.FindAsync(url.ID);
                    if (urlDb != null)
                    {
                        urlDb.UrlLarga = url.UrlLarga;
                        //urlDb.UrlCorta = url.UrlCorta;
                        urlDb.FechaExpira = url.FechaExpira;
                        urlDb.FechaModificacion = DateTime.Now;
                        urlDb.Habilitado = url.Habilitado;
                        urlDb.CodProducto = url.CodProducto;
                        urlDb.Producto = url.Producto;
                        urlDb.Observaciones = url.Observaciones;
                    }
                }
                await _db.SaveChangesAsync();                              

                urlDb = await _db.UrlConfigs.FindAsync(url.ID);
                if (urlDb != null)
                    urlDb.UrlCorta = url.UrlChunk;
                await _db.SaveChangesAsync();
                _db.Database.CommitTransaction();
                return RedirectToAction("Index");
            }
            return PartialView(url);
        }

        #region Api
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos(string? filtro)
        {
            ICollection<UrlConfigs>? lstUrl = null;
            if (filtro == null)
            {
                lstUrl = await _db.UrlConfigs.OrderByDescending(o => o.FechaCreacion).Take(5000).ToListAsync();
            }
            else
            {
                lstUrl = await _db.UrlConfigs
                    .Where(w => w.CodProducto.Contains(filtro) || w.Producto.Contains(filtro)
                    || w.UrlCorta.Contains(filtro) || w.UrlLarga.Contains(filtro)
                    )
                    .OrderByDescending(o => o.FechaCreacion).Take(5000).ToListAsync();
            }
            return Json(new { result = "Ok", data = lstUrl });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var urlDb = await _db.UrlConfigs.FindAsync(id);
            if (urlDb == null)
            {
                return Json(new { success = false, message = "Error al borrar, no se encontró el registro." });
            }
            else
            {
                _db.UrlConfigs.Remove(urlDb);
                await _db.SaveChangesAsync();
            }
            return Json(new { success = true, message = "Registro eliminado con éxito" });
        }
        #endregion
    }
}
