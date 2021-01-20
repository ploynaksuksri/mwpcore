using System;
using System.Threading.Tasks;
using Shouldly;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace Mwp.Form
{
    public sealed class FormsDomainTests : MwpDomainTestBase
    {
        public FormsDomainTests()
        {
            _formRepository = GetRequiredService<IRepository<Form>>();
            _submissionRepository = GetRequiredService<IRepository<Submission>>();
        }

        private readonly IRepository<Form> _formRepository;
        private readonly IRepository<Submission> _submissionRepository;

        private static readonly Guid FormId = Guid.NewGuid();

        [Fact]
        public async Task Should_Save_Form()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                var form = new Form(FormId)
                {
                    Name = "TestForm00",
                    Data = "{}"
                };
                await _formRepository.InsertAsync(form);
            });

            await WithUnitOfWorkAsync(async () =>
            {
                var savedForm = await _formRepository.FindAsync(x => x.Name == "TestForm00");
                savedForm.ShouldNotBeNull();
                savedForm.Id.ShouldBe(FormId);
                savedForm.Data.ShouldBe("{}");
            });
        }

        [Fact]
        public async Task Should_Save_Submission()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                var form = new Form(FormId)
                {
                    Name = "TestForm01",
                    Data = "{}"
                };
                await _formRepository.InsertAsync(form);
            });

            var submissionId = Guid.NewGuid();
            await WithUnitOfWorkAsync(async () =>
            {
                var submission = new Submission(submissionId, FormId)
                {
                    Data = "{}"
                };
                await _submissionRepository.InsertAsync(submission);
            });

            await WithUnitOfWorkAsync(async () =>
            {
                var savedSubmission = await _submissionRepository
                    .FindAsync(x => x.Id == submissionId);
                savedSubmission.ShouldNotBeNull();
                savedSubmission.Id.ShouldBe(submissionId);
                savedSubmission.FormId.ShouldBe(FormId);
                savedSubmission.Data.ShouldBe("{}");
            });
        }
    }
}