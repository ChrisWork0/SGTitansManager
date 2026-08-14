using Microsoft.EntityFrameworkCore;
using SGTitansManager.Models;
using SGTitansManager.Server.Database;
using SGTitansManager.Server.Helper;

namespace SGTitansManager.Server.Repositories;

public class UserRepository : Repository<User>
{
    private readonly DbSet<User> DbSet;
    
    public UserRepository(ManagerContext dbContext) : base(dbContext)
    {
        DbSet = dbContext.Set<User>();
    }

    public async Task<User?> LoginByUserName(string username, string passwordHash) =>
        await DbSet.FirstOrDefaultAsync(u => u.UserName == username 
                                             && u.PasswordHash == passwordHash 
                                             && u.IsActive);

    public async Task<ResultDto> DeactivateUser(Guid userId, bool isActive)
    {
        var user = await DbSet.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
            return Result.UnSuccess(404, "User not found");
        user.IsActive = isActive;
        await Save();
        return Result.Success($"Successfully changed user to active = {isActive}");
    }
}