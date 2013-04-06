using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Elastacloud.AzureManagement.Fluent.Clients;
using Elastacloud.AzureManagement.Fluent.Helpers;
using Elastacloud.AzureManagement.Fluent.Linq;
using Elastacloud.AzureManagement.Fluent.Types;

namespace Elastacloud.FluentExamples
{
    public class WorkflowLinqToStorage : IWorkflow
    {
        private readonly X509Certificate2 _managementCertificate;
        private readonly string _subscriptionId;
        private string _accountName;

        public WorkflowLinqToStorage(string subscriptionId, X509Certificate2 managementCertificate)
        {
            _subscriptionId = subscriptionId;
            _managementCertificate = managementCertificate;
        }

        #region Implementation of IWorkflow

        public void PreActionSteps()
        {
            // get a random account name
            var namer = new RandomAccountName();
            _accountName = namer.GetPureRandomValue();

            var client = new StorageClient(_subscriptionId, _managementCertificate);
            client.CreateNewStorageAccount(_accountName);
            Console.WriteLine("Created new storage account with name {0}", _accountName);
        }

        public void DoWork()
        {
            var inputs = new LinqToAzureInputs()
            {
                ManagementCertificateThumbprint = _managementCertificate.Thumbprint,
                SubscriptionId = _subscriptionId
            };
            var queryableStorage = new LinqToAzureOrderedQueryable<StorageAccount>(inputs);
            // build up a filtered query to check the new account
            var query = from account in queryableStorage
                        where account.Name == _accountName
                        select account;
            var myAccount = query.First();

            // check that the account count is 1
            Console.WriteLine("{0} record returned", query.Count());
            Console.WriteLine("Name: {0}", myAccount.Name);
            Console.WriteLine("Url: {0}", myAccount.Url);
            Console.WriteLine("Primary Key: {0}", myAccount.PrimaryAccessKey);
            Console.WriteLine("Secondary Key: {0}", myAccount.SecondaryAccessKey);

            // do an unfiltered query and get the names of all of the storage accounts
            var unfilteredQuery = from account in queryableStorage
                                  select account;
            int storageCount = unfilteredQuery.Count();
            var take = unfilteredQuery.Take(storageCount / 2);
            // bring back some info we can use
            Console.WriteLine("You have {0} storage accounts", storageCount);
            var storageAccounts = take.ToList();
            foreach (var storageAccount in storageAccounts)
                Console.WriteLine("Account name: {0}", storageAccount.Name);
        }

        public void PostActionSteps()
        {
            var client = new StorageClient(_subscriptionId, _managementCertificate);
            client.DeleteStorageAccount(_accountName);

            Console.WriteLine("Deleted storage account with name {0}", _accountName);
        }

        #endregion
    }
}
