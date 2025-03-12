
using Microsoft.AspNetCore.Mvc;

namespace APIInventario.API.Controllers
{
    [Route("api/reporte")]
    [ApiController]
    public class ReporteController : ControllerBase
    {
        private readonly ReporteService _reporteService;

        public ReporteController(ReporteService reporteService)
        {
            _reporteService = reporteService;
        }

        [HttpGet("productos-inventario-bajo")]
        public IActionResult ObtenerReporteProductosInventarioBajo()
        {
            try
            {
                var reportePdf = _reporteService.GenerarReporteProductosInventarioBajo();

                return File(reportePdf, "application/pdf", "reporte_productos_inventario_bajo.pdf");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al generar el reporte: {ex.Message}");
            }
        }

    }
}
