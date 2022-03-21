using Microsoft.AspNetCore.WebUtilities;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shortener.Models 
{  
    public class UrlConfigs
    {
        [Key]
        public int ID { get; set; }
        [Display(Name = "URL Larga")]
        [MaxLength(3000)]
        [Required(ErrorMessage = "Debe ingresar una url válida.")]
        public string? UrlLarga { get; set; }

        [MaxLength(10)]
        [DisplayName("Shortener")]
        [Required(ErrorMessage = "No se generó una url corta")]
        public string UrlCorta { get; set; }

        [DisplayName("Fecha creación")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd'T'HH:mm:ss}", ApplyFormatInEditMode = true)] // para visualizar bien la fecha en el date picker del html5
        public DateTime? FechaCreacion { get; set; }

        [DisplayName("Fecha modificación")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd'T'HH:mm:ss}", ApplyFormatInEditMode = true)] // para visualizar bien la fecha en el date picker del html5
        public DateTime? FechaModificacion { get; set; }

        [DisplayName("Fecha expiración")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd'T'HH:mm:ss}", ApplyFormatInEditMode = true)] // para visualizar bien la fecha en el date picker del html5
        public DateTime? FechaExpira { get; set; }

        public bool Habilitado { get; set; }

        [DisplayName("N° Visitas")]
        public long NumVisitas { get; set; }

        [DisplayName("Cód. Producto")]
        [MaxLength(20)]
        [Required(ErrorMessage = "Debe ingresar el código del producto.")]
        public string? CodProducto { get; set; }

        [MaxLength(100)]
        [Required(ErrorMessage = "Debe ingresar la descripción del producto.")]
        public string? Producto { get; set; }


        [MaxLength(300)]
        public string? Observaciones { get; set; }
        public string UrlChunk => WebEncoders.Base64UrlEncode(BitConverter.GetBytes(ID));
        
    }
}
