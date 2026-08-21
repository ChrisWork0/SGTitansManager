using SGTitansManager.Models;
using SGTitansManager.Server.Database;

namespace SGTitansManager.Server.Repositories;

public class PlayerRepository(ManagerContext dbContext) : Repository<Player>(dbContext);