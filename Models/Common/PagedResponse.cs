namespace Models.Common;

public record PaginationParams(
    int PageNumber = 1,
    int PageSize = 10
);

public record PagedResponse<T>(
    IEnumerable<T> Items,
    int TotalCount,
    int TotalPages,
    int PageNumber,
    int PageSize
)
{
    public bool HasNextPage => PageNumber < TotalPages;
    public bool HasPreviousPage => PageNumber > 1;
}
