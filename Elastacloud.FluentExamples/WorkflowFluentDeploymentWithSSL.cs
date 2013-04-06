using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Elastacloud.AzureManagement.Fluent;
using Elastacloud.AzureManagement.Fluent.Clients;
using Elastacloud.AzureManagement.Fluent.Helpers;
using Elastacloud.AzureManagement.Fluent.Linq;
using Elastacloud.AzureManagement.Fluent.Services;
using Elastacloud.AzureManagement.Fluent.Types;

namespace Elastacloud.FluentExamples
{
    public class WorkflowFluentDeploymentWithSSL : IWorkflow
    {
        private readonly X509Certificate2 _managementCertificate;
        private readonly string _subscriptionId;
        private string _accountName;
        private IServiceTransaction _context;

        public WorkflowFluentDeploymentWithSSL(string subscriptionId, X509Certificate2 managementCertificate)
        {
            _subscriptionId = subscriptionId;
            _managementCertificate = managementCertificate;
        }

        #region Implementation of IWorkflow

        public void PreActionSteps()
        {
            Console.WriteLine("Doing everything in work!");
            var manager = new SubscriptionManager(_subscriptionId);
            _context = manager.GetDeploymentManager()
                                 .AddCertificate(_managementCertificate)
                                 .ForNewDeployment("stacked deployment")
                                 .SetBuildDirectoryRoot("builddirectory")
                                 .EnableRemoteDesktopAndSslForRole("ExampleWebRole")
                                 .WithUsernameAndPassword("azurecoder", "Password101!")
                                 .GenerateAndAddServiceCertificate("stackedliverpoolpaas")
                                 .WithNewHostedService("stackedliverpoolpaas")
                                 .WithStorageAccount("stackedstorage")
                                 .AddDescription("Created by fluent management")
                                 .AddEnvironment(DeploymentSlot.Production)
                                 .AddLocation(LocationConstants.NorthEurope)
                                 .AddParams(DeploymentParams.StartImmediately)
                                 .WaitUntilAllRoleInstancesAreRunning()
                                 .Go();
        }

        public void DoWork()
        {
            Console.WriteLine(!_context.Commit()
                                  ? "unable to deploy cloud service - have rolled back"
                                  : "completed and deployed cloud service");
        }

        public void PostActionSteps()
        {
            Console.WriteLine("Finishing stuff here!");
        }

        #endregion
    }
}
