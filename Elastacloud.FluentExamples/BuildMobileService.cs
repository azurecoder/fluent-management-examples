using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Elastacloud.AzureManagement.Fluent.Clients;
using Elastacloud.AzureManagement.Fluent.Types.MobileServices;

namespace Elastacloud.FluentExamples
{
    public class BuildMobileService : IBuilder
    {
        public BuildMobileService(string subscriptionId, X509Certificate2 managementCertificate)
        {
            SubscriptionId = subscriptionId;
            ManagementCertificate = managementCertificate;
        }

        #region Implementation of IBuilder

        public void SpinUp()
        {
            var client = new MobileServiceClient(SubscriptionId, ManagementCertificate);
            client.CreateMobileServiceWithNewDb(Settings.MobileServiceName, Settings.Username, Settings.Password);
            Console.WriteLine("Yeah, we created a mobile service using the Service Management API - even though it hasn't been published yet!");

            // Display some details about the mobile service
            Console.WriteLine("Application key: {0}", client.ApplicationKey);
            Console.WriteLine("Application url: {0}", client.ApplicationUrl);
            Console.WriteLine("Description: {0}", client.Description);
            Console.WriteLine("Location: {0}", client.Location);
            Console.WriteLine("Master key: {0}", client.MasterKey);
            Console.WriteLine("Mobile service Db name: {0}", client.MobileServiceDbName);
            Console.WriteLine("Mobile service server name: {0}", client.MobileServiceSqlName);
            Console.WriteLine("Sql Azure Db name: {0}", client.SqlAzureDbName);
            Console.WriteLine("Sql Azure server name: {0}", client.SqlAzureServerName);
            Console.WriteLine("Mobile service state: {0}", client.MobileServiceState.ToString());
        }

        public void TearDown()
        {
            var client = new MobileServiceClient(SubscriptionId, ManagementCertificate, Settings.MobileServiceName)
                {
                    SqlAzureUsername = Settings.Username,
                    SqlAzurePassword = Settings.Password
                };
            
            client.Delete();
        }

        public void DoSomething()
        {
            var client = new MobileServiceClient(SubscriptionId, ManagementCertificate, Settings.MobileServiceName);
            if (!client.Tables.Exists(a => a.TableName == "Speakers"))
                client.AddTable("Speakers");
            client.AddTableScript(CrudOperation.Insert, "Speakers", "function insert(item, user, request){ /*Another fluent success!*/request.execute();}", Roles.User);
            client.FacebookClientId = "test";
            client.FacebookClientSecret = "test";
            client.GoogleClientId = "test";
            client.GoogleClientSecret = "test";
            client.TwitterClientId = "test";
            client.TwitterClientSecret = "test";
            client.DynamicSchemaEnabled = true;
            client.MicrosoftAccountClientId = "test";
            client.MicrosoftAccountClientSecret = "test";
            client.MicrosoftAccountPackageSID = "test";
            client.Update();

            Console.WriteLine("Application key: {0}", client.ApplicationKey);
            Console.WriteLine("Application url: {0}", client.ApplicationUrl);
            Console.WriteLine("Description: {0}", client.Description);
            Console.WriteLine("Location: {0}", client.Location);
            Console.WriteLine("Master key: {0}", client.MasterKey);
            Console.WriteLine("Mobile service Db name: {0}", client.MobileServiceDbName);
            Console.WriteLine("Mobile service server name: {0}", client.MobileServiceSqlName);
            Console.WriteLine("Sql Azure Db name: {0}", client.SqlAzureDbName);
            Console.WriteLine("Sql Azure server name: {0}", client.SqlAzureServerName);
            Console.WriteLine("Mobile service state: {0}", client.MobileServiceState.ToString());
            foreach (var table in client.Tables)
            {
                Console.WriteLine("==================================================");
                Console.WriteLine("Table name: {0}", table.TableName);
                Console.WriteLine("Size: {0} bytes", table.SizeInBytes);
                Console.WriteLine("Row count: {0}", table.NumberOfRecords);
                Console.WriteLine("No of indexes: {0}", table.NumberOfIndexes);
                Console.WriteLine("Read permission: {0}", table.ReadPermission);
                Console.WriteLine("Insert permission: {0}", table.InsertPermission);
                Console.WriteLine("Delete permission: {0}", table.DeletePermission);
                Console.WriteLine("Update permission: {0}", table.UpdatePermission);
                Console.WriteLine("==================================================");
            }

            var logs = client.Logs;
            Console.WriteLine("There are {0} log entries", logs.Count);
            Console.WriteLine("There are {0} error log entries", logs.Count(a => a.Type == LogLevelType.Error));
            Console.WriteLine("There are {0} warning log entries", logs.Count(a => a.Type == LogLevelType.Warning));
            Console.WriteLine("There are {0} information log entries", logs.Count(a => a.Type == LogLevelType.Information));
            client.RegenerateKeys();
            client.Restart();
        }

        public string SubscriptionId { get; private set; }
        public X509Certificate2 ManagementCertificate { get; private set; }

        #endregion

        public override string ToString()
        {
            return "BuildMobileService";
        }
    }
}
