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
        return entities.Select(e => new ClientResponse(e.Id, e.Name, e.Phone, e.Address));
    }

    public async Task<ClientResponse?> GetByIdAsync(int id)
    {
        var e = await repository.GetByIdAsync(id);
        return e == null ? null : new ClientResponse(e.Id, e.Name, e.Phone, e.Address);
    }

    public async Task<ClientResponse> CreateAsync(ClientRequest request)
    {
        var entity = new Client
        {
            Name = request.Name,
            Phone = request.Phone,
            Address = request.Address
        };
        await repository.AddAsync(entity);
        await repository.SaveChangesAsync();
        return new ClientResponse(entity.Id, entity.Name, entity.Phone, entity.Address);
    }

    public async Task<bool> UpdateAsync(int id, ClientRequest request)
    {
        var entity = await repository.GetByIdAsync(id);
        if (entity == null) return false;

        entity.Name = request.Name;
        entity.Phone = request.Phone;
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
}
