using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Elastacloud.AzureManagement.Fluent;
using Elastacloud.AzureManagement.Fluent.Clients;
using Elastacloud.AzureManagement.Fluent.Helpers;
using Elastacloud.AzureManagement.Fluent.Linq;
using Elastacloud.AzureManagement.Fluent.Types;

namespace Elastacloud.FluentExamples
{
    public class WorkflowRoleSystemWatcher : IWorkflow
    {
        private readonly X509Certificate2 _managementCertificate;
        private readonly string _subscriptionId;
        private string _accountName;
        private ManualResetEvent _reset = new ManualResetEvent(false);

        public WorkflowRoleSystemWatcher(string subscriptionId, X509Certificate2 managementCertificate)
        {
            _subscriptionId = subscriptionId;
            _managementCertificate = managementCertificate;
        }

        #region Implementation of IWorkflow

        public void PreActionSteps()
        {
            // get a random account name
            Console.WriteLine("Starting watching role for changes!");
        }

        public void DoWork()
        {
            var manager = new SubscriptionManager(_subscriptionId);
            var watcher = manager.GetRoleStatusChangedWatcher("stackedliverpoolpaas", "ExampleWebRole", DeploymentSlot.Production,
                                                _managementCertificate.Thumbprint);
            watcher.RoleStatusChangeHandler += (status, oldStatus) =>
                                                   {
                                                       if (oldStatus == RoleStatus.Unknown)
                                                           return;
                                                       Console.WriteLine("status has changed from {0} to {1}", oldStatus,
                                                                         status);
                                                       _reset.Set();
                                                   };
            _reset.WaitOne();
        }

        public void PostActionSteps()
        {
            Console.WriteLine("Finished watching role");
        }

        #endregion

        public override string ToString()
        {
            return "WorkflowRoleSystemWatcher";
        }
    }
}
