using DbModel.Tables;
using IBusiness;
using IRepository;
using Models.Clients;

namespace Business;

public class ClientBusiness(IClientRepository repository) : IClientBusiness
{
    public async Task<IEnumerable<ClientResponse>> GetAllAsync()
    {
        var entities = await repository.GetAllAsync();
        return entities.Select(MapToResponse);
    }

    public async Task<ClientResponse?> GetByIdAsync(int id)
    {
        var e = await repository.GetByIdAsync(id);
        return e == null ? null : MapToResponse(e);
    }

    public async Task<ClientResponse> CreateAsync(ClientRequest request)
    {
        ValidateDocument(request.DocumentType, request.DocumentNumber);

        var entity = new Client
        {
            Name = request.Name,
            Phone = request.Phone,
            DocumentType = NormalizeDocumentType(request.DocumentType),
            DocumentNumber = NormalizeDocumentNumber(request.DocumentNumber),
            Address = request.Address
        };
        await repository.AddAsync(entity);
        await repository.SaveChangesAsync();
        return MapToResponse(entity);
    }

    public async Task<bool> UpdateAsync(int id, ClientRequest request)
    {
        var entity = await repository.GetByIdAsync(id);
        if (entity == null) return false;

        ValidateDocument(request.DocumentType, request.DocumentNumber);

        entity.Name = request.Name;
        entity.Phone = request.Phone;
        entity.DocumentType = NormalizeDocumentType(request.DocumentType);
        entity.DocumentNumber = NormalizeDocumentNumber(request.DocumentNumber);
        entity.Address = request.Address;

        repository.Update(entity);
        return await repository.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await repository.GetByIdAsync(id);
        if (entity == null) return false;

        repository.Remove(entity);
        return await repository.SaveChangesAsync() > 0;
    }

    private static ClientResponse MapToResponse(Client client) =>
        new(client.Id, client.Name, client.Phone, client.DocumentType, client.DocumentNumber, client.Address);

    private static void ValidateDocument(string? documentType, string? documentNumber)
    {
        var normalizedType = NormalizeDocumentType(documentType);
        var normalizedNumber = NormalizeDocumentNumber(documentNumber);

        if (string.IsNullOrWhiteSpace(normalizedType) && string.IsNullOrWhiteSpace(normalizedNumber))
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(normalizedType) || string.IsNullOrWhiteSpace(normalizedNumber))
        {
            throw new InvalidOperationException("El cliente debe tener tipo y numero de documento.");
        }

        if (!IsSupportedDocumentType(normalizedType))
        {
            throw new InvalidOperationException("Tipo de documento no soportado. Use DNI o RUC.");
        }

        if (!normalizedNumber.All(char.IsLetterOrDigit))
        {
            throw new InvalidOperationException("El numero de documento solo debe contener letras y numeros.");
        }

        if (normalizedType == "DNI" && normalizedNumber.Length != 8)
        {
            throw new InvalidOperationException("El DNI debe tener 8 digitos.");
        }

        if (normalizedType == "RUC" && normalizedNumber.Length != 11)
        {
            throw new InvalidOperationException("El RUC debe tener 11 digitos.");
        }
    }

    private static bool IsSupportedDocumentType(string documentType) =>
        documentType is "DNI" or "RUC";

    private static string? NormalizeDocumentType(string? documentType) =>
        string.IsNullOrWhiteSpace(documentType) ? null : documentType.Trim().ToUpperInvariant();

    private static string? NormalizeDocumentNumber(string? documentNumber) =>
        string.IsNullOrWhiteSpace(documentNumber) ? null : documentNumber.Trim().ToUpperInvariant();
}
