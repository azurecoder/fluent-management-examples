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
    public class WorkflowFluentDeployment : IWorkflow
    {
        private readonly X509Certificate2 _managementCertificate;
        private readonly string _subscriptionId;
        private string _accountName;
        private ServiceOrchestrator _orchestrator;

        public WorkflowFluentDeployment(string subscriptionId, X509Certificate2 managementCertificate)
        {
            _subscriptionId = subscriptionId;
            _managementCertificate = managementCertificate;
        }

        #region Implementation of IWorkflow

        public void PreActionSteps()
        {
            _orchestrator = new ServiceOrchestrator();
            // get a random account name
            var namer = new RandomAccountName();
            _accountName = namer.GetPureRandomValue();
            // create the storage 
            var manager = new SubscriptionManager(_subscriptionId);
            var serviceTransactionStorage = manager.GetStorageManager()
                   .CreateNew(_accountName)
                   .AddCertificate(_managementCertificate)
                   .WithDescription("Created by fluent management- for stacked")
                   .WithLocation(LocationConstants.NorthEurope)
                   .Go();
            _orchestrator.AddDeploymentStep(serviceTransactionStorage);
            // create the sql azure
            var serviceTransactionSql = manager.GetSqlAzureManager()
                   .AddNewServer(LocationConstants.NorthEurope)
                   .AddCertificate(_managementCertificate)
                   .AddNewFirewallRuleForWindowsAzureHostedService()
                   .AddNewFirewallRuleWithMyIp("my ip rule - for stacked")
                   .WithSqlAzureCredentials("azurecoder", "Password101!")
                   .AddNewDatabase("stacked")
                   .AddNewDatabaseAdminUser("azurecoder_db", "Password101!")
                   .Go();
            _orchestrator.AddDeploymentStep(serviceTransactionSql);
            var serviceTransactionDeployment = manager.GetDeploymentManager()
                    .AddCertificate(_managementCertificate)
                    .ForNewDeployment("stacked deployment")
                    .SetCspkgEndpoint(@"C:\Users\Richard\Documents\Fluent Examples\Elastacloud.FluentExamples\TestCloudInstall\bin\Release\app.publish")
                    .WithNewHostedService("stackedliverpoolpaas")
                    .WithStorageAccount(_accountName)
                    .AddDescription("Fluent management @ Stacked")
                    .AddEnvironment(DeploymentSlot.Production)
                    .AddLocation(LocationConstants.NorthEurope)
                    .AddParams(DeploymentParams.StartImmediately|DeploymentParams.WarningsAsErrors)
                    .ForRole("ExampleWebRole")
                    .WithInstanceCount(4)
                    .WaitUntilAllRoleInstancesAreRunning()
                    .Go();
            _orchestrator.AddDeploymentStep(serviceTransactionDeployment);
        }

        public void DoWork()
        {
            if (!_orchestrator.Commit())
            {
                Console.WriteLine("deployment failed had to roll back!");
            }
        }

        public void PostActionSteps()
        {
            var inputs = new LinqToAzureInputs()
            {
                ManagementCertificateThumbprint = _managementCertificate.Thumbprint,
                SubscriptionId = _subscriptionId
            };
            // build up a filtered query to check the new account
            var cloudServiceQueryable = new LinqToAzureOrderedQueryable<CloudService>(inputs);
            // get only production deployments
            var query = from service in cloudServiceQueryable
                        where service.Deployments.Count != 0 
                        && service.Deployments.Any(a => a.Slot == DeploymentSlot.Production)
                        && service.Deployments.Any(a => a.TotalRoleInstanceCount == 4)
                        select service;

            var count = query.Count();
            Console.WriteLine("Number returned: {0}", count);

            if (count == 0)
            {
                Console.WriteLine("Something has gone wrong with the linq to cloud services!");
                return;
            }
            Console.WriteLine("Service name: {0}", query.FirstOrDefault().Name);
        }

        #endregion
    }
}
