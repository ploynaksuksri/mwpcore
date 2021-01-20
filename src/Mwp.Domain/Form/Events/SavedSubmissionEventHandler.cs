using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities.Events;
using Volo.Abp.EventBus;

namespace Mwp.Form.Events
{
    public class SavedSubmissionEventHandler : ILocalEventHandler<EntityUpdatedEventData<Submission>>,
        ILocalEventHandler<EntityCreatedEventData<Submission>>,
        ITransientDependency
    {
        private readonly IFormStorageClient _formStorageClient;

        public SavedSubmissionEventHandler(IFormStorageClient formStorageClient)
        {
            _formStorageClient = formStorageClient;
        }

        public async Task HandleEventAsync(EntityUpdatedEventData<Submission> eventData)
        {
            await _formStorageClient.SaveSubmissionHistory(eventData.Entity);
        }

        public async Task HandleEventAsync(EntityCreatedEventData<Submission> eventData)
        {
            await _formStorageClient.SaveSubmissionHistory(eventData.Entity);
        }
    }
}