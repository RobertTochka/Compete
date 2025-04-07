using Compete_POCO_Models.Infrastrcuture.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;

namespace compete_poco.Infrastructure.Services
{
    public abstract class CRepository
    {
        protected readonly ApplicationContext _ctx;

        public CRepository(ApplicationContext ctx)
        {
            _ctx = ctx;
        }
        public async Task SaveChangesAsync() => await _ctx.SaveChangesAsync();

        public EntityEntry Remove(object entity) => _ctx.Remove(entity);
        public void ClearCache() => _ctx.ChangeTracker.Clear();
        public async Task<IDbTransaction> BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            if(_ctx.Database.CurrentTransaction == null)
                return (await _ctx.Database.BeginTransactionAsync(isolationLevel)).GetDbTransaction();
            return _ctx.Database.CurrentTransaction.GetDbTransaction();
         }

    }
}
