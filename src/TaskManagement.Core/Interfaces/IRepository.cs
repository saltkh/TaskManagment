namespace TaskManagement.Core.Interfaces;

/// <summary>
/// Generic repository contract — every entity gets Create/Read/Update/Delete for free.
/// TEntity = the entity class (User, Project, etc.)
/// </summary>
public interface IRepository<TEntity> where TEntity : class
{
    Task<TEntity?> GetByIdAsync(int id);
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task<TEntity> CreateAsync(TEntity entity);
    Task<TEntity> UpdateAsync(TEntity entity);
    Task DeleteAsync(TEntity entity);
    Task<bool> ExistsAsync(int id);
}
