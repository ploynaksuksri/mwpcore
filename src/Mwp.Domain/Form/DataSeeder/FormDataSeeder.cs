using System;
using System.Collections.Generic;
using System.Linq;
using Mwp.DataSeeder;
using Mwp.Form.Repository;
using Mwp.Utilities;
using Newtonsoft.Json.Linq;
using Volo.Abp.MultiTenancy;

namespace Mwp.Form.DataSeeder
{
    public class FormDataSeeder : MwpDataSeederBase<IFormRepository, Form>
    {
        private readonly ICurrentTenant _currentTenant;

        public FormDataSeeder(IFormRepository repository, ICurrentTenant currentTenant)
            : base(repository)
        {
            _currentTenant = currentTenant;
        }

        protected override List<Form> BuildSeedingRecords()
        {
            var sampleForms = new List<Form>();
            var resourceFileNames = new[]
            {
                "AcceptanceOfAppointmentOrReappointment",
                "AccountsDisclosureChecklistSummary",
                "DemoAdvCalculateValue",
                "DemoCalculateProperty",
                "GroupRiskAssessment",
                "Materiality",
                "PlanningChecklist",
                "PreliminaryAnalyticalReview_Financial",
                "PreliminaryAnalyticalReview_Ratios",
                "TrialBalance",
                "C4DividendReceivedWorksheet",
                "C7RentalPropertyWorksheet",
                "G2LoanRepaymentScheduleWorksheet",
                "F8Division7ARepaymentWorksheet",
                "C2LivestockWorksheet",
                "G2AsSubForm"
            };

            var sharedSubForm = CreateFormFromResourceName("SubForm-Status");
            sampleForms.Add(sharedSubForm);
            foreach (var resourceFileName in resourceFileNames)
            {
                var form = CreateForm(resourceFileName, sharedSubForm);
                sampleForms.Add(form);
            }

            var wizard = CreateFormFromResourceName("WizardForm");
            sampleForms.Add(wizard);

            return sampleForms;
        }

        protected override Form GetExistingRecord(List<Form> existingRecords, Form record)
        {
            return existingRecords.FirstOrDefault(e => e.Name == record.Name);
        }

        private Form CreateForm(
            string resourceFileName,
            Form sharedSubForm)
        {
            var form = CreateFormFromResourceName(resourceFileName);
            if (form.Data.IndexOf("5d936d4abef254181cda9415", StringComparison.Ordinal) >= 0)
            {
                form.Data = form.Data.Replace("5d936d4abef254181cda9415", sharedSubForm.Id.ToString());
            }

            return form;
        }

        private Form CreateFormFromResourceName(string resourceFileName)
        {
            var jsonFromFile = MwpEmbeddedResourceUtils.ReadStringFromEmbededResource(
                GetType().Assembly,
                $"Mwp.Form.DataSeeder.json.{resourceFileName}.json");
            var id = Guid.NewGuid();
            var jObj = JObject.Parse(jsonFromFile);
            jObj["_id"] = id;
            var form = new Form(id)
            {
                Data = jObj.ToString(),
                Name = jObj.Value<string>("title"),
                CreationTime = DateTime.Now,
                TenantId = _currentTenant?.Id
            };
            return form;
        }
    }
}