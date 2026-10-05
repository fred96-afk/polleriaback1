using Models.Clients;

namespace IBusiness;

public interface IClientBusiness
{
    Task<IEnumerable<ClientResponse>> GetAllAsync();
    Task<ClientResponse?> GetByIdAsync(int id);
    Task<ClientResponse> CreateAsync(ClientRequest request);
    Task<bool> UpdateAsync(int id, ClientRequest request);
    Task<bool> DeleteAsync(int id);
}
