using System;
using System.Linq;
using System.Threading.Tasks;
using Mwp.Wopi;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.EventBus;
using Volo.Abp.Uow;

namespace Mwp.File.Events
{
    public class DeletedFileIndexEventHandler : ILocalEventHandler<DeletedFileIndexEventData>, ITransientDependency
    {
        private readonly IRepository<WopiFile> _wopiFileRepository;
        protected readonly IUnitOfWorkManager UnitOfWorkManager;

        public DeletedFileIndexEventHandler(IRepository<WopiFile> wopiFileRepository, IUnitOfWorkManager manager)
        {
            _wopiFileRepository = wopiFileRepository;
            UnitOfWorkManager = manager;
        }

        [UnitOfWork]
        public async Task HandleEventAsync(DeletedFileIndexEventData eventData)
        {
            using (var unitOfWork = UnitOfWorkManager.Begin())
            {
                if (eventData.FileIds != null && eventData.FileIds.Count > 0)
                {
                    var fileIds = eventData.FileIds.Select(id => new Guid(id));
                    await _wopiFileRepository.DeleteAsync(f => fileIds.Contains(f.Id), true);
                    await unitOfWork.CompleteAsync();
                }
            }
        }
    }
}