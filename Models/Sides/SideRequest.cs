using DbModel.Tables;

namespace Models.Sides;

public record SideRequest(
    string Name,
    string? Description,
    decimal Price,
    SideType Type
);
