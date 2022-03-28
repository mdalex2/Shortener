using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRCoder;
using Shortener.DbConection;
using Shortener.Models;
using System.DrawingCore;
using static QRCoder.PayloadGenerator;

namespace Shortener.Controllers
{
    [Route("{controller=Home}/{action=Index}/{id?}")]
    //[Authorize(Roles = "Web Sistemas")]
    
    public class AcortadorController : Controller
    {
        private readonly DbConex _db;
        private readonly IConfiguration _config;
        private static Random random = new Random();

        public AcortadorController(DbConex db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }
        public async Task<string> ObtenerCodigoAleatorio(int longitud = 0)
        {
            const string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            long contLoop = 0;

            int longShortener = (longitud == 0) ? Convert.ToInt32(_config.GetSection("LongitudShortener").Value) : longitud;
            bool existeCodigoDb = true;
            string codigoRandom = "";
            while (existeCodigoDb == true && contLoop <= long.MaxValue)
            {
                //genero un ramdom en base a los caracteres parametrizados
                codigoRandom = new string(Enumerable.Repeat(characters, longShortener).Select(s => s[random.Next(s.Length)]).ToArray());

                existeCodigoDb = await _db.UrlShorts.AnyAsync(s => EF.Functions.Collate(s.UrlCorta, "SQL_Latin1_General_CP1_CS_AS") == codigoRandom && s.Eliminado == false);
                contLoop++;
            }
            return codigoRandom;
        }
        public async Task <IActionResult> Index()
        {
            return View();
        }
        public async Task<IActionResult> Upsert(int? id)
        {
            UrlShort? url = new UrlShort();
            if (id == null)
            {
                return View(url);
            }
            url = await _db.UrlShorts.FindAsync(id.GetValueOrDefault());
            if (url == null)
            {
                return NotFound();
            }            
            return PartialView(url);
        }
        //public async Task<IActionResult> QrImagenBase64(int id)
        //{
        //    var urlShort = await _db.UrlShorts.FindAsync(id);
        //    if (urlShort == null)
        //    {
        //        return NotFound();
        //    }
        //    //string fullUrl = _config.GetSection("ServidorShortener").Value.ToString() + urlShort.UrlCorta;
        //    QRCoder.PayloadGenerator.Url fullUrl = new QRCoder.PayloadGenerator.Url(_config.GetSection("ServidorShortener").Value.ToString() + urlShort.UrlCorta);
        //    QRCodeGenerator qrGenerator = new QRCodeGenerator();

        //    QRCodeData qrCodeData = qrGenerator.CreateQrCode(fullUrl, QRCodeGenerator.ECCLevel.Q);
        //    PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);

        //    byte[] qrCodeImage = qrCode.GetGraphic(20);
        //    string model = Convert.ToBase64String(qrCodeImage);
        //    ViewBag.fullUrl = fullUrl;
        //    return View("QrImagenBase64", model);
        //}

        public async Task<IActionResult> QrImagenSvg(int id)
        {            
            var urlShort = await _db.UrlShorts.FindAsync(id);
            if (urlShort == null)
            {
                return NotFound();
            }

            string fullUrl = _config.GetSection("ServidorShortener").Value.ToString() + urlShort.UrlCorta;
            
            QRCodeGenerator qrGenerator = new QRCodeGenerator();

            QRCodeData qrCodeData = qrGenerator.CreateQrCode(fullUrl, QRCodeGenerator.ECCLevel.Q);
            SvgQRCode qrCode = new SvgQRCode(qrCodeData);
            
            string qrCodeAsSvg = qrCode.GetGraphic(20);
            var folder = Path.Combine(Environment.CurrentDirectory, "wwwroot", "Images","QR");
            var fullFileName = Path.Combine(folder,urlShort.UrlCorta+".svg");
            if (!Directory.Exists(folder))
            {
                DirectoryInfo di = Directory.CreateDirectory(folder);
            }
            System.IO.File.WriteAllText(fullFileName, qrCodeAsSvg);
            ViewBag.fullUrl = fullUrl;
            return View("QrImagenBase64", urlShort);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(UrlShort url)
        {            
            if (ModelState.IsValid)
            {
                UrlShort? urlDb = null;
                

                await _db.Database.BeginTransactionAsync();

                if (url.Id == 0)
                {
                    //si es nuevo genero un nuevo codigo
                    string codigoAleatorio = await ObtenerCodigoAleatorio();              

                    url.UrlCorta = codigoAleatorio;
                    url.FechaCreacion = DateTime.Now;
                    url.FechaModificacion = DateTime.Now;
                    await _db.UrlShorts.AddAsync(url);                    
                }
                else
                {
                    urlDb = await _db.UrlShorts.FindAsync(url.Id);
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

                urlDb = await _db.UrlShorts.FindAsync(url.Id);
                
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
            ICollection<UrlShort>? lstUrl = null;
            if (filtro == null)
            {
                lstUrl = await _db.UrlShorts.Where(w => w.Eliminado == false).OrderByDescending(o => o.FechaCreacion).Take(200).ToListAsync();
            }
            else
            {
                lstUrl = await _db.UrlShorts
                    .Where(w =>  (w.CodProducto.Contains(filtro) || w.Producto.Contains(filtro)
                    || w.UrlCorta.Contains(filtro) || w.UrlLarga.Contains(filtro)) && w.Eliminado == false
                    )
                    .OrderByDescending(o => o.FechaCreacion).Take(5000).ToListAsync();
            }
            return Json(new { result = "Ok", data = lstUrl });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var urlDb = await _db.UrlShorts.FindAsync(id);
            if (urlDb == null)
            {
                return Json(new { success = false, message = "Error al borrar, no se encontró el registro." });
            }
            else
            {
                urlDb.Eliminado = true;
                await _db.SaveChangesAsync();
            }
            return Json(new { success = true, message = "Registro eliminado con éxito" });
        }
        #endregion
    }
}
