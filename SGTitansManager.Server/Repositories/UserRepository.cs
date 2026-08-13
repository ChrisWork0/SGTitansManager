using Microsoft.EntityFrameworkCore;
using SGTitansManager.Models;
using SGTitansManager.Server.Database;

namespace SGTitansManager.Server.Repositories;

public class UserRepository : Repository<User>
{
    private readonly DbSet<User> DbSet;
    
    public UserRepository(ManagerContext dbContext) : base(dbContext)
    {
        DbSet = dbContext.Set<User>();
    }
}