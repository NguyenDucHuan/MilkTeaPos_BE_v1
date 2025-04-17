namespace MilkTeaPosManagement.DAL.GenericRepositories;

public interface IGenericRepositoryFactory
{
    IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : class;
}
