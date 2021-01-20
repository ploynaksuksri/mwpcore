using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace Mwp.DataSeeder
{
    public abstract class MwpDataSeederBase<TRepository, TEntity, TKey> : IMwpDataSeeder<TRepository, TEntity, TKey>, ITransientDependency
        where TEntity : class, IEntity<TKey>
        where TRepository : IRepository<TEntity, TKey>
    {
        protected readonly TRepository Repository;

        protected List<TEntity> SeedingRecords;

        protected MwpDataSeederBase(TRepository repository)
        {
            Repository = repository;
            Initialize();
        }

        private void Initialize()
        {
            SeedingRecords = BuildSeedingRecords();
        }

        protected abstract List<TEntity> BuildSeedingRecords();

        protected abstract TEntity GetExistingRecord(List<TEntity> existingRecords, TEntity record);

        public virtual async Task SeedData()
        {
            var existingRecords = await Repository.GetListAsync();

            foreach (var seedingRecord in SeedingRecords.Where(r => GetExistingRecord(existingRecords, r) == null))
            {
                await Repository.InsertAsync(seedingRecord, true);
            }
        }
    }

    public abstract class MwpDataSeederBase<TRepository, TEntity> : IMwpDataSeeder<TRepository, TEntity>, ITransientDependency
        where TEntity : class, IEntity<Guid>
        where TRepository : IRepository<TEntity>
    {
        protected readonly TRepository Repository;

        protected List<TEntity> SeedingRecords;

        protected MwpDataSeederBase(TRepository repository)
        {
            Repository = repository;
            Initialize();
        }

        private void Initialize()
        {
            SeedingRecords = BuildSeedingRecords();
        }

        protected abstract List<TEntity> BuildSeedingRecords();

        protected abstract TEntity GetExistingRecord(List<TEntity> existingRecords, TEntity record);

        public virtual async Task SeedData()
        {
            var existingRecords = await Repository.GetListAsync();

            foreach (var seedingRecord in SeedingRecords.Where(r => GetExistingRecord(existingRecords, r) == null))
            {
                await Repository.InsertAsync(seedingRecord, true);
            }
        }
    }
}