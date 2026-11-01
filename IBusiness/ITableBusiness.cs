using Models.Tables;

namespace IBusiness;

public interface ITableBusiness
{
    Task<IEnumerable<TableResponse>> GetAllAsync(string? mapId = null);
    Task<TableResponse?> GetByIdAsync(int id);
    Task<TableResponse?> GetByNumberAsync(string number);
    Task<TableResponse> CreateAsync(TableRequest request);
    Task<bool> UpdateAsync(int id, TableRequest request);
    Task<bool> UpdatePositionAsync(int id, TablePositionRequest request);
    Task<bool> DeleteAsync(int id);

    /// <summary>
    /// Notifica en tiempo real que un comensal está llamando al mozo.
    /// </summary>
    Task<bool> NotifyCallWaiterAsync(string tableNumber, string? message = null);

    /// <summary>
    /// Notifica en tiempo real que un comensal solicitó la cuenta.
    /// </summary>
    Task<bool> NotifyRequestBillAsync(string tableNumber, string? message = null);

    /// <summary>
    /// Emite una alerta genérica asociada a una mesa.
    /// </summary>
    Task<bool> NotifyTableAlertAsync(string tableNumber, string? message = null, string type = "table_alert");
}
