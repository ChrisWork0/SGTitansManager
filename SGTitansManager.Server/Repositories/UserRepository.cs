using System.Numerics;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using SGTitansManager.Models;
using SGTitansManager.Server.Database;
using SGTitansManager.Server.Helper;
using SGTitansManager.Server.Services;

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

    public override Task<ResultDto> Patch(Guid id, JsonPatchDocument updates)
    {
        foreach (var operation in updates.Operations)
        {
            if (operation.path == "/passwordHash")
                operation.value = (operation.value.ToString() 
                                   ?? throw new InvalidOperationException()).Sha256();
        }
        return base.Patch(id, updates);
    }
}