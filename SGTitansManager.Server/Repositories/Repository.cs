using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using SGTitansManager.Models;
using SGTitansManager.Server.Database;
using SGTitansManager.Server.Dtos;
using SGTitansManager.Server.Helper;

namespace SGTitansManager.Server.Repositories;

public class Repository<TModel> where TModel : BaseModel
{
    private readonly ManagerContext _dbContext;
    private readonly DbSet<TModel> _dbSet;

    protected Repository(ManagerContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = _dbContext.Set<TModel>();
    }

    public virtual async Task<List<TModel>> Get(bool withDeleted = false)
    {
        if (!withDeleted)
            return await _dbSet.Where(m => m.Deleted == null).ToListAsync();
        return await _dbSet.ToListAsync();
    }

    private async Task<TModel?> GetById(Guid id) 
        => await _dbSet.FirstOrDefaultAsync(m => m.Id == id);
    
    public virtual async Task<TModel?> GetByIdWithIncludes(Guid id, string[]? includes = null)
    {
        IQueryable<TModel> set = _dbSet;
        if (includes == null)
            return await GetById(id);
        foreach (var include in includes)
        {
            if (typeof(TModel).GetProperties().All(p => p.Name != include))
                throw new ArgumentOutOfRangeException();
            set = set.Include(include);
        }
        return await set.FirstOrDefaultAsync(m => m.Id == id);
    }

    public virtual TModel Add(TModel model)
    {
        _dbSet.Add(model);
        return model;
    }

    public virtual void AddList(List<TModel> models)
        => _dbSet.AddRange(models);
    
    public virtual async Task AddAndSave(TModel model)
    {
        await _dbSet.AddAsync(model);
        await _dbContext.SaveChangesAsync();
    }

    public virtual async Task<int> Count()
        => await _dbSet.CountAsync();

    public virtual async Task<ResultDto> Patch(Guid id, JsonPatchDocument<TModel> updates)
    {
        var model = await GetById(id);
        if (model == null)
            return Result.UnSuccess(404);
        try
        {
            updates.ApplyTo(model);
            await _dbContext.SaveChangesAsync();
            return Result.Success(model);
        }
        catch (Exception e)
        {
            return Result.UnSuccess(400, e.Message);
        }
    }
    
    public async Task Save() => await _dbContext.SaveChangesAsync();

    public virtual async Task<bool> Delete(Guid id)
    {
        var model = await GetById(id);
        if (model == null)
            return false;
        model.Deleted = DateTime.Now.ToUniversalTime();
        await  _dbContext.SaveChangesAsync();
        return true;
    }

    public virtual async Task<bool> HardDelete(Guid id)
    {
        var model = await GetById(id);
        if (model == null)
            return false;
        _dbSet.Remove(model);
        await _dbContext.SaveChangesAsync();
        return true;
    }
}