using Models.Catalogos;

namespace IBusiness;

public interface ICatalogoBusiness
{
    Task<IEnumerable<TipoDocumentoResponse>> GetTiposDocumentoAsync();
    Task<IEnumerable<MetodoPagoResponse>> GetMetodosPagoAsync();
    Task<IEnumerable<TipoComprobanteResponse>> GetTiposComprobanteAsync();
    Task<IEnumerable<EstadoPedidoResponse>> GetEstadosPedidoAsync();
    Task<IEnumerable<CargoResponse>> GetCargosAsync();
    Task<IEnumerable<CategoriaProductoResponse>> GetCategoriasProductoAsync();
}
