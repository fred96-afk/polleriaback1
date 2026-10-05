using IBusiness;
using Models.Catalogos;

namespace Business;

public class CatalogoBusiness : ICatalogoBusiness
{
    public Task<IEnumerable<TipoDocumentoResponse>> GetTiposDocumentoAsync()
    {
        IEnumerable<TipoDocumentoResponse> items =
        [
            new(1, "DNI"),
            new(6, "RUC")
        ];

        return Task.FromResult(items);
    }

    public Task<IEnumerable<MetodoPagoResponse>> GetMetodosPagoAsync()
    {
        IEnumerable<MetodoPagoResponse> items =
        [
            new(1, "Efectivo", "Pago en efectivo", true),
            new(2, "Tarjeta", "Pago con tarjeta", true),
            new(3, "Yape", "Pago con Yape", true),
            new(4, "Mercado Pago", "Pago online con Mercado Pago", true)
        ];

        return Task.FromResult(items);
    }

    public Task<IEnumerable<TipoComprobanteResponse>> GetTiposComprobanteAsync()
    {
        IEnumerable<TipoComprobanteResponse> items =
        [
            new(1, "Factura", true),
            new(2, "Boleta", true)
        ];

        return Task.FromResult(items);
    }

    public Task<IEnumerable<EstadoPedidoResponse>> GetEstadosPedidoAsync()
    {
        IEnumerable<EstadoPedidoResponse> items =
        [
            new(1, "Pendiente"),
            new(2, "En preparacion"),
            new(3, "En camino"),
            new(4, "Entregado"),
            new(5, "Anulado")
        ];

        return Task.FromResult(items);
    }

    public Task<IEnumerable<CargoResponse>> GetCargosAsync()
    {
        IEnumerable<CargoResponse> items =
        [
            new(1, "Admin", "Administrador del sistema", true),
            new(2, "Waiter", "Mozo", true),
            new(3, "Delivery", "Repartidor", true),
            new(4, "Client", "Cliente", true)
        ];

        return Task.FromResult(items);
    }

    public Task<IEnumerable<CategoriaProductoResponse>> GetCategoriasProductoAsync()
    {
        IEnumerable<CategoriaProductoResponse> items =
        [
            new(1, "Pollos", "Pollos a la brasa y combos", true),
            new(2, "Bebidas", "Gaseosas y refrescos", true),
            new(3, "Complementos", "Papas, ensaladas y extras", true)
        ];

        return Task.FromResult(items);
    }
}
