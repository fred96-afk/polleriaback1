using DbModel.Tables;
using IBusiness;
using IRepository;
using MapsterMapper;
using Models.Tables;

namespace Business;

public class TableBusiness(
    ITableRepository repository,
    IMapper mapper,
    IPusherService pusherService) : ITableBusiness
{
    public async Task<IEnumerable<TableResponse>> GetAllAsync(string? mapId = null)
    {
        var entities = await repository.GetAllAsync();
        if (!string.IsNullOrWhiteSpace(mapId))
        {
            entities = entities.Where(t => t.MapId.Equals(mapId, StringComparison.OrdinalIgnoreCase));
        }
        return mapper.Map<IEnumerable<TableResponse>>(entities.OrderBy(t => t.Number));
    }

    public async Task<TableResponse?> GetByIdAsync(int id)
    {
        var entity = await repository.GetByIdAsync(id);
        return entity == null ? null : mapper.Map<TableResponse>(entity);
    }

    public async Task<TableResponse?> GetByNumberAsync(string number)
    {
        var entity = await repository.GetByNumberAsync(number);
        return entity == null ? null : mapper.Map<TableResponse>(entity);
    }

    public async Task<TableResponse> CreateAsync(TableRequest request)
    {
        var entity = mapper.Map<RestaurantTable>(request);
        if (string.IsNullOrWhiteSpace(entity.Name))
        {
            entity.Name = $"Mesa {entity.Number}";
        }
        entity.CreatedAt = DateTime.UtcNow;

        await repository.AddAsync(entity);
        await repository.SaveChangesAsync();

        return mapper.Map<TableResponse>(entity);
    }

    public async Task<bool> UpdateAsync(int id, TableRequest request)
    {
        var entity = await repository.GetByIdAsync(id);
        if (entity == null) return false;

        entity.Number = request.Number;
        entity.Name = string.IsNullOrWhiteSpace(request.Name) ? $"Mesa {request.Number}" : request.Name;
        entity.Chairs = request.Chairs;
        entity.Location = request.Location;
        entity.MapId = request.MapId;
        entity.Shape = request.Shape;
        entity.X = request.X;
        entity.Y = request.Y;
        entity.IsActive = request.IsActive;
        entity.UpdatedAt = DateTime.UtcNow;

        repository.Update(entity);
        await repository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdatePositionAsync(int id, TablePositionRequest request)
    {
        var entity = await repository.GetByIdAsync(id);
        if (entity == null) return false;

        entity.X = request.X;
        entity.Y = request.Y;
        entity.UpdatedAt = DateTime.UtcNow;

        repository.Update(entity);
        await repository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await repository.GetByIdAsync(id);
        if (entity == null) return false;

        repository.Remove(entity);
        await repository.SaveChangesAsync();
        return true;
    }

    public Task<bool> NotifyCallWaiterAsync(string tableNumber, string? message = null)
        => SendNotificationAsync(
            tableNumber,
            eventName: "call-waiter",
            type: "call_waiter",
            defaultMessage: "El comensal está solicitando atención del mozo.",
            message: message);

    public Task<bool> NotifyRequestBillAsync(string tableNumber, string? message = null)
        => SendNotificationAsync(
            tableNumber,
            eventName: "request-bill",
            type: "request_bill",
            defaultMessage: "El comensal ha solicitado la cuenta.",
            message: message);

    public Task<bool> NotifyTableAlertAsync(string tableNumber, string? message = null, string type = "table_alert")
        => SendNotificationAsync(
            tableNumber,
            eventName: "table-alert",
            type: string.IsNullOrWhiteSpace(type) ? "table_alert" : type,
            defaultMessage: "Se ha registrado una alerta en la mesa.",
            message: message);

    private async Task<bool> SendNotificationAsync(
        string tableNumber,
        string eventName,
        string type,
        string defaultMessage,
        string? message)
    {
        if (string.IsNullOrWhiteSpace(tableNumber)) return false;

        var rawNumber = tableNumber.Trim();
        var entity = await repository.GetByNumberAsync(rawNumber);

        if (entity == null)
        {
            var normalizedNumber = NormalizeTableNumber(rawNumber);
            if (!string.Equals(normalizedNumber, rawNumber, StringComparison.OrdinalIgnoreCase))
            {
                entity = await repository.GetByNumberAsync(normalizedNumber);
            }
        }

        if (entity == null) return false;

        var notification = new TableNotificationEvent
        {
            TableId = entity.Id,
            TableNumber = entity.Number,
            TableName = entity.Name,
            Type = type,
            Message = string.IsNullOrWhiteSpace(message) ? defaultMessage : message.Trim(),
            Timestamp = DateTime.UtcNow
        };

        await pusherService.TriggerAsync("tables", eventName, notification);
        return true;
    }

    private static string NormalizeTableNumber(string tableNumber)
    {
        if (string.IsNullOrWhiteSpace(tableNumber)) return string.Empty;

        var normalized = tableNumber.Trim();
        if (normalized.StartsWith("Mesa ", StringComparison.OrdinalIgnoreCase))
        {
            normalized = normalized[5..].Trim();
        }
        else if (normalized.StartsWith("M-", StringComparison.OrdinalIgnoreCase))
        {
            normalized = normalized[2..].Trim();
        }

        return normalized;
    }
}
