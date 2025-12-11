using DataAccessLayer.Context;
using DataAccessLayer.Repositories.Impementations;
using DataAccessLayer.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;


namespace DataAccessLayer.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly RecyclingDbContext _context;
        private IDbContextTransaction? _transaction;

        public IOrderRepository Orders { get; private set; }
        public IMaterialRepository Materials { get; private set; }
        public IFactoryRepository Factories { get; private set; }
        public IRewardRepository Rewards { get; private set; }
        public IApplicationUserRepository Users { get; private set; }

        public UnitOfWork(RecyclingDbContext context)
        {
            _context = context;
            Orders = new OrderRepository(_context);
            Materials = new MaterialRepository(_context);
            Factories = new FactoryRepository(_context);
            Rewards = new RewardRepository(_context);
            Users = new ApplicationUserRepository(_context);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await SaveChangesAsync();
                if (_transaction != null)
                {
                    await _transaction.CommitAsync();
                }
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}
