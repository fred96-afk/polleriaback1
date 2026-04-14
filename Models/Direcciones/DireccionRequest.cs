namespace Models.Direcciones;

public record DireccionRequest(
    int? IdCliente,
    int? IdPedido,
    string Ubicacion,
    string? Referencia
);
