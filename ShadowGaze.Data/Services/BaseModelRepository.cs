using BigBroPublicBot.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace BigBroPublicBot.Data.Services;

public abstract class BaseModelRepository<TModel> : IDisposable
    where TModel : BaseDatabaseModel
{
    protected abstract DbSet<TModel> Table { get; }
    protected DatabaseContext DatabaseContext { get; private set; } = new();

    private bool _disposeContext = true;


    /// <summary>
    /// Repository will use specified <see cref="DbContext"/> instance and will not dispose DbContext 
    /// </summary>
    /// <param name="context">Database Context instance</param>
    public void UseContext(DatabaseContext context)
    {
        DatabaseContext = context;
        _disposeContext = false;
    }

    /// <summary>
    /// Update or create specified model 
    /// </summary>
    /// <param name="model">Model to save</param>
    /// <returns>Saved model with all generated fields lke Id, CreatedAt, etc.</returns>
    public virtual TModel Save(TModel model)
    {
        model = Table.Update(model).Entity;
        DatabaseContext.SaveChanges();
        return model;
    }
    
    /// <summary>
    /// Update or create specified model 
    /// </summary>
    /// <param name="model">Model to save</param>
    /// <returns>Saved model with all generated fields lke Id, CreatedAt, etc.</returns>
    public virtual async Task<TModel> SaveAsync(TModel model)
    {
        model = Table.Update(model).Entity;
        await DatabaseContext.SaveChangesAsync();
        return model;
    }

    public virtual TModel GetById(Guid id)
    {
        return Table.FirstOrDefault(model => model.Id == id);
    }

    public virtual Task<TModel> GetByIdAsync(Guid id)
    {
        return Table.FirstOrDefaultAsync(model => model.Id == id);
    }

    /// <summary>
    /// Get IQueryable <see cref="T:TModel"/> collection for custom queries 
    /// </summary>
    /// <returns>Table object typed to IQueryable</returns>
    public virtual IQueryable<TModel> AsQueryable()
    {
        return Table.AsQueryable();
    }

    public void Dispose()
    {
        if (!_disposeContext)
        {
            return;
        }

        DatabaseContext.Dispose();
    }
}