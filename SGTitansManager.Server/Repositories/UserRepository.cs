using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using SGTitansManager.Models;
using SGTitansManager.Server.Database;
using SGTitansManager.Server.Dtos;
using SGTitansManager.Server.Helper;
using SGTitansManager.Server.Services;

namespace SGTitansManager.Server.Repositories;

public class UserRepository : Repository<AppUser>
{
    private readonly DbSet<AppUser> _dbSet;
    private readonly DbSet<Member> _dbSetMember;
    
    public UserRepository(ManagerContext dbContext) : base(dbContext)
    {
        _dbSet = dbContext.Set<AppUser>();
        _dbSetMember = dbContext.Set<Member>();
    }

    public async Task<List<AppUser>> GetUsers(bool withDeleted = false, bool? active = null)
    {
        if (active == null)
        {
            if (!withDeleted)
                return await _dbSet.Where(u => u.Deleted == null).ToListAsync();
            return await _dbSet.ToListAsync();
        }
        else
        {
            if (!withDeleted)
                return await _dbSet.Where(u => u.Deleted == null && u.IsActive == active).ToListAsync();
            return await _dbSet.Where(u => u.IsActive == active).ToListAsync();
        }
    }

    public async Task<AppUser?> GetUserByPlayerId(Guid playerId)
    {
        var member = await _dbSetMember.FirstOrDefaultAsync(m => m.PlayerId == playerId);
        if (member == null)
            return null;
        var user = await _dbSet.FirstOrDefaultAsync(u => u.MemberId == member.Id);
        if (user == null)
            return null;
        return user;
    }

    public async Task<AppUser?> GetUserWithMemberAndPlayer(Guid userId)
    {
        var user = await _dbSet.Include(u => u.Member).ThenInclude(m => m.Player)
            .FirstOrDefaultAsync(u => u.Id == userId);
        return user;
    }
    
    public async Task<AppUser?> LoginByUserName(string username, string passwordHash) =>
        await _dbSet.Include(u => u.Member).FirstOrDefaultAsync(u => u.UserName == username 
                                             && u.PasswordHash == passwordHash 
                                             && u.IsActive);

    public override async Task<ResultDto> Patch(Guid id, JsonPatchDocument<AppUser> updates)
    {
        foreach (var operation in updates.Operations)
        {
            if (operation.path == "/passwordHash")
                return Result.UnSuccess(403, "Forbid");
        }
        return await base.Patch(id, updates);
    }

    public async Task<bool> SetPasswordAsync(Guid userId, string newPassword)
    {
        var user = await _dbSet.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
            return false;
        user.PasswordHash = newPassword.Sha256();
        user.RecoveryCode = null;
        await Save();
        return true;
    }

    public async Task<AppUser?> GetUserByRecoveryCode(string recoveryCode)
    {
        var user = await _dbSet.Include(u => u.Member)
            .FirstOrDefaultAsync(u => u.RecoveryCode == recoveryCode);
        if (user == null)
            return null;
        return user;
    }
        
    
}