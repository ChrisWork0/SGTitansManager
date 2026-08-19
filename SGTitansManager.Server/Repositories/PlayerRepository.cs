using SGTitansManager.Models;
using SGTitansManager.Server.Database;

namespace SGTitansManager.Server.Repositories;

public class PlayerRepository : Repository<Player>
{
    public PlayerRepository(ManagerContext dbContext) : base(dbContext)
    {
        
    }
}