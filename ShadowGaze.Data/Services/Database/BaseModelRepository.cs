using Microsoft.EntityFrameworkCore;
using ShadowGaze.Data.Models.Database;

namespace ShadowGaze.Data.Services.Database;

public abstract class BaseModelRepository<TModel> : IDisposable
    where TModel : BaseDatabaseModel
{
    protected abstract DbSet<TModel> Table { get; }

    protected DatabaseContext DatabaseContext
    {
        get => _context ?? new DatabaseContext();
        private set => _context = value;
    }

    private DatabaseContext _context;

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
    /// Update or create a specified model 
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

    public virtual TModel GetById(int id)
    {
        return Table.FirstOrDefault(model => model.Id == id);
    }

    public virtual Task<TModel> GetByIdAsync(int id)
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