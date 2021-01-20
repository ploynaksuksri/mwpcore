using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace Mwp.DataSeeder
{
    public interface IMwpDataSeeder<TRepository, TEntity, TKey>
        where TEntity : class, IEntity<TKey>
        where TRepository : IRepository<TEntity, TKey>
    {
        Task SeedData();
    }

    public interface IMwpDataSeeder<TRepository, TEntity>
        where TEntity : class, IEntity<Guid>
        where TRepository : IRepository<TEntity>
    {
        Task SeedData();
    }
}