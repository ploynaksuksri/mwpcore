using System.Threading.Tasks;
using Mwp.DataSeeder;
using Mwp.Form.Repository;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;

namespace Mwp.Form
{
    public class FormDataSeedContributor : IDataSeedContributor, ITransientDependency
    {
        private readonly IMwpDataSeeder<IFormRepository, Form> _formDataSeeder;

        public FormDataSeedContributor(IMwpDataSeeder<IFormRepository, Form> formDataSeeder)
        {
            _formDataSeeder = formDataSeeder;
        }

        [UnitOfWork]
        public async Task SeedAsync(DataSeedContext context)
        {
            await _formDataSeeder.SeedData();
        }
    }
}