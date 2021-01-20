using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities.Events;
using Volo.Abp.EventBus;

namespace Mwp.Form.Events
{
    public class SavedFormEventHandler : ILocalEventHandler<EntityUpdatedEventData<Form>>,
        ILocalEventHandler<EntityCreatedEventData<Form>>,
        ITransientDependency
    {
        private readonly IFormStorageClient _formStorageClient;

        public SavedFormEventHandler(IFormStorageClient formStorageClient)
        {
            _formStorageClient = formStorageClient;
        }

        public async Task HandleEventAsync(EntityUpdatedEventData<Form> eventData)
        {
            await _formStorageClient.SaveFormHistory(eventData.Entity);
        }

        public async Task HandleEventAsync(EntityCreatedEventData<Form> eventData)
        {
            await _formStorageClient.SaveFormHistory(eventData.Entity);
        }
    }
}