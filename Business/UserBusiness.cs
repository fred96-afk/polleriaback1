using AutoMapper;
using DbModel.Tables;
using IBusiness;
using IBusiness.Security;
using IRepository;
using Models.Users;

namespace Business;

public class UserBusiness(
    IUserRepository repository, 
    IMapper mapper, 
    IPasswordHasher passwordHasher) : IUserBusiness
{
    public async Task<IEnumerable<UserResponse>> GetAllAsync()
    {
        var entities = await repository.GetAllAsync();
        return mapper.Map<IEnumerable<UserResponse>>(entities);
    }

    public async Task<UserResponse?> GetByIdAsync(int id)
    {
        var e = await repository.GetByIdAsync(id);
        return mapper.Map<UserResponse>(e);
    }

    public async Task<UserResponse> CreateAsync(UserRequest request)
    {
        var entity = mapper.Map<User>(request);
        
        // Hashear la contraseña con salt+pepper antes de guardar
        entity.PasswordHash = passwordHasher.HashPassword(request.Password);

        await repository.AddAsync(entity);
        await repository.SaveChangesAsync();
        
        return mapper.Map<UserResponse>(entity);
    }

    public async Task<bool> UpdateAsync(int id, UserRequest request)
    {
        var entity = await repository.GetByIdAsync(id);
        if (entity == null) return false;

        mapper.Map(request, entity);

        if (!string.IsNullOrEmpty(request.Password))
        {
            entity.PasswordHash = passwordHasher.HashPassword(request.Password);
        }

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
