using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using APIInventario.Infrastructure.Data;


public class ReporteService
{
    private readonly ApplicationDbContext _context;

    public ReporteService(ApplicationDbContext context)
    {
        _context = context;
    }

    public byte[] GenerarReporteProductosInventarioBajo()
    {
        using (var ms = new MemoryStream())
        {
            var pdfWriter = new PdfWriter(ms);
            var pdfDocument = new PdfDocument(pdfWriter);
            var document = new Document(pdfDocument);

            PdfFont fontBold = PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA_BOLD);
            PdfFont fontNormal = PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA);

            document.Add(new Paragraph($"Fecha de generación: {DateTime.Now:dd/MM/yyyy}")
                .SetFont(fontNormal)
                .SetFontSize(12)
                .SetFontColor(ColorConstants.DARK_GRAY)
                .SetTextAlignment(TextAlignment.RIGHT));

            document.Add(new Paragraph("Reporte de Productos con Inventario Bajo")
                .SetFont(fontBold)
                .SetFontSize(20)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetMarginBottom(10));

            document.Add(new Paragraph("Chanchito Feliz")
                .SetFont(fontBold)
                .SetFontSize(16)
                .SetFontColor(ColorConstants.BLUE)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetMarginBottom(20));

            float[] columnWidths = { 2, 5, 2, 3, 4 };
            Table table = new Table(UnitValue.CreatePercentArray(columnWidths)).UseAllAvailableWidth();

            var headerCellStyle = new Style()
                .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(12)
                .SetFontColor(ColorConstants.BLACK)
                .SetFont(fontBold);

            table.AddHeaderCell(new Cell().Add(new Paragraph("ID")).AddStyle(headerCellStyle));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Producto")).AddStyle(headerCellStyle));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Cantidad")).AddStyle(headerCellStyle));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Precio")).AddStyle(headerCellStyle));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Fecha Ingreso")).AddStyle(headerCellStyle));

            var rowStyle = new Style()
                .SetFontSize(10)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFont(fontNormal);

            var productosInventarioBajo = _context.Productos
                .Where(p => p.Cantidad < 5)
                .ToList();

            if (productosInventarioBajo.Count == 0)
            {
                document.Add(new Paragraph("\n No hay productos con stock bajo.")
                    .SetFont(fontBold)
                    .SetFontSize(14)
                    .SetFontColor(ColorConstants.GREEN)
                    .SetTextAlignment(TextAlignment.CENTER));
            }
            else
            {
                foreach (var producto in productosInventarioBajo)
                {
                    table.AddCell(new Cell().Add(new Paragraph(producto.Id.ToString())).AddStyle(rowStyle));
                    table.AddCell(new Cell().Add(new Paragraph(producto.Nombre)).AddStyle(rowStyle));
                    table.AddCell(new Cell().Add(new Paragraph(producto.Cantidad.ToString())).AddStyle(rowStyle));
                    table.AddCell(new Cell().Add(new Paragraph($"S/. {producto.Precio:F2}")).AddStyle(rowStyle));
                    table.AddCell(new Cell().Add(new Paragraph(producto.FechaCreacion.ToString("dd/MM/yyyy"))).AddStyle(rowStyle));
                }

                document.Add(table);
            }

            document.Add(new Paragraph("Reporte generado automáticamente")
                .SetFontSize(10)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontColor(ColorConstants.GRAY)
                .SetMarginTop(30));

            document.Close();
            return ms.ToArray();
        }
    }
}
