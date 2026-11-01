namespace Models.Tables;

public record TableResponse(
    int Id,
    string Number,
    string Name,
    int Chairs,
    string Location,
    string MapId,
    string Shape,
    double X,
    double Y,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);
