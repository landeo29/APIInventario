using APIInventario.Core.Models;
using DinkToPdf;
using DinkToPdf.Contracts;
using System.Text;

namespace APIInventario.Infrastructure.Services
{
    public class PdfService
    {
        private readonly IConverter _converter;

        public PdfService(IConverter converter)
        {
            _converter = converter;
        }

        public byte[] GenerarReporteProductosBajoInventario(List<Producto> productos)
        {
            var sb = new StringBuilder();

            sb.Append("<h2>Reporte de Productos con Inventario Bajo</h2>");
            sb.Append("<table border='1' cellpadding='5' cellspacing='0'>");
            sb.Append("<tr><th>Nombre</th><th>Descripción</th><th>Cantidad</th><th>Precio</th></tr>");

            foreach (var p in productos)
            {
                sb.Append($"<tr><td>{p.Nombre}</td><td>{p.Descripcion}</td><td>{p.Cantidad}</td><td>{p.Precio:C}</td></tr>");
            }

            sb.Append("</table>");

            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.A4
                },
                Objects = {
                    new ObjectSettings() {
                        PagesCount = true,
                        HtmlContent = sb.ToString(),
                        WebSettings = { DefaultEncoding = "utf-8" }
                    }
                }
            };

            return _converter.Convert(doc);
        }
    }
}
