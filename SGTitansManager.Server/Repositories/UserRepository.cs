using System.Numerics;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using SGTitansManager.Models;
using SGTitansManager.Server.Database;
using SGTitansManager.Server.Helper;
using SGTitansManager.Server.Services;

namespace SGTitansManager.Server.Repositories;

public class UserRepository : Repository<User>
{
    private readonly DbSet<User> DbSet;
    private readonly DbSet<Member> DbSetMember;
    
    public UserRepository(ManagerContext dbContext) : base(dbContext)
    {
        DbSet = dbContext.Set<User>();
        DbSetMember = dbContext.Set<Member>();
    }

    public async Task<User?> GetUserByPlayerId(Guid playerId)
    {
        var member = await DbSetMember.FirstOrDefaultAsync(m => m.PlayerId == playerId);
        if (member == null)
            return null;
        var user = await DbSet.FirstOrDefaultAsync(u => u.MemberId == member.Id);
        if (user == null)
            return null;
        return user;
    }
    
    public async Task<User?> LoginByUserName(string username, string passwordHash) =>
        await DbSet.FirstOrDefaultAsync(u => u.UserName == username 
                                             && u.PasswordHash == passwordHash 
                                             && u.IsActive);

    public override Task<ResultDto> Patch(Guid id, JsonPatchDocument<User> updates)
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