using IBusiness;
using Microsoft.AspNetCore.Mvc;

namespace Polleria.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CatalogosController(ICatalogoBusiness business) : ControllerBase
{
    [HttpGet("tipos-documento")]
    public async Task<IActionResult> GetTiposDocumento() => Ok(await business.GetTiposDocumentoAsync());

    [HttpGet("metodos-pago")]
    public async Task<IActionResult> GetMetodosPago() => Ok(await business.GetMetodosPagoAsync());

    [HttpGet("tipos-comprobante")]
    public async Task<IActionResult> GetTiposComprobante() => Ok(await business.GetTiposComprobanteAsync());

    [HttpGet("estados-pedido")]
    public async Task<IActionResult> GetEstadosPedido() => Ok(await business.GetEstadosPedidoAsync());

    [HttpGet("cargos")]
    public async Task<IActionResult> GetCargos() => Ok(await business.GetCargosAsync());

    [HttpGet("categorias-producto")]
    public async Task<IActionResult> GetCategoriasProducto() => Ok(await business.GetCategoriasProductoAsync());
}
