using Microsoft.EntityFrameworkCore;
using MilkTeaPosManagement.DAL.GenericRepositories;

namespace MilkTeaPosManagement.DAL.UnitOfWorks
{
    public interface IUnitOfWork : IGenericRepositoryFactory, IDisposable
    {
        int Commit();

        Task<int> CommitAsync();

        Task BeginTransactionAsync();

        Task CommitTransactionAsync();

        Task RollbackTransactionAsync();
    }

    public interface IUnitOfWork<TContext> : IUnitOfWork where TContext : DbContext
    {
        TContext Context { get; }
    }
}
