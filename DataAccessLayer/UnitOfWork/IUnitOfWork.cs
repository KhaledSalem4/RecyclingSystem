using DataAccessLayer.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IOrderRepository Orders { get; }
        IMaterialRepository Materials { get; }
        IFactoryRepository Factories { get; }
        IRewardRepository Rewards { get; }
        IHistoryRewardRepository HistoryRewards { get; }
        IApplicationUserRepository Users { get; }

        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
        
        // ✅ ADD: Get execution strategy
        IExecutionStrategy CreateExecutionStrategy();

    }
}
