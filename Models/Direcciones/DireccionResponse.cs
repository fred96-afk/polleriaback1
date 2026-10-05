namespace Models.Direcciones;

public record DireccionResponse(
    int IdDireccion,
    int? IdCliente,
    int? IdPedido,
    string? Ubicacion
);
