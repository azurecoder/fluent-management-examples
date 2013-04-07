using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Elastacloud.AzureManagement.Fluent.Clients;
using Elastacloud.AzureManagement.Fluent.Helpers;
using Elastacloud.AzureManagement.Fluent.Helpers.PublishSettings;
using Elastacloud.AzureManagement.Fluent.Linq;
using Elastacloud.AzureManagement.Fluent.Types;

namespace Elastacloud.FluentExamples
{
    public class WorkflowPublishSettings : IWorkflow
    {
        public string SubscriptionId { get; private set; }
        public X509Certificate2 ManagementCertificate { get; private set; }
        public List<Subscription> Subscriptions { get; private set; }
        public string PublishSettingsPath { get; private set; }
        public double SchemaVersion { get; private set; }


        public WorkflowPublishSettings(string subscriptionId, string publishSettings)
        {
            Settings.SubscriptionId = SubscriptionId = subscriptionId;
            PublishSettingsPath = publishSettings;
        }

        #region Implementation of IWorkflow

        public void PreActionSteps()
        {
            var extractor = new PublishSettingsExtractor(PublishSettingsPath);
            Subscriptions = extractor.GetSubscriptions();
            Settings.ManagementCertificate = ManagementCertificate = extractor.AddPublishSettingsToPersonalMachineStore();
            SchemaVersion = extractor.SchemaVersion;
        }

        public void DoWork()
        {
            foreach (var subscription in Subscriptions)
                Console.WriteLine("Subscription name: {0}, id: {1}", subscription.Name, subscription.Id);
        }

        public void PostActionSteps()
        {
            Console.WriteLine("Finished read .publishsetting file with schema version {0}", SchemaVersion);
        }

        #endregion
    }
}
