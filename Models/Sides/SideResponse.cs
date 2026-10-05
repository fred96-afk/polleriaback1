using DbModel.Tables;

namespace Models.Sides;

public record SideResponse(
    int Id,
    string Name,
    string? Description,
    decimal Price,
    SideType Type
);
