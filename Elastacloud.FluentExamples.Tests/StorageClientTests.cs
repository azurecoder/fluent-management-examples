using System.Configuration;
using System.Security.Cryptography.X509Certificates;
using Elastacloud.AzureManagement.Fluent.Clients;
using Elastacloud.AzureManagement.Fluent.Helpers.PublishSettings;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Elastacloud.FluentExamples.Tests
{
	[TestClass]
	public class StorageClientTests
	{
		static string publishSettingsFile = @"F:\Code\Fluent Management Examples Forked\noopman.publishsettings";
		static string subscriptionId = "b951dbc2-b9f2-415d-baeb-fdd1ee98f85e";
		const string storageAccountName = "apicatest";

		static PublishSettingsExtractor settings;
		static X509Certificate2 certificate;

		StorageClient storageClient;

		[ClassInitialize]
		public static void ReadConfig(TestContext context)
		{
			publishSettingsFile = ConfigurationManager.AppSettings["Publish Settings File"];
			Assert.IsNotNull(publishSettingsFile);

			subscriptionId = ConfigurationManager.AppSettings["Subscription ID"];
			Assert.IsNotNull(subscriptionId);
		
			settings = PublishSettingsExtractor.GetFromFile(publishSettingsFile);
			Assert.IsNotNull(settings);

			certificate = settings.AddPublishSettingsToPersonalMachineStore();
			Assert.IsNotNull(certificate);
		}

		[TestInitialize]
		public void CreateStorageClient()
		{
			storageClient = new StorageClient(subscriptionId, certificate);

			Assert.IsNotNull(storageClient);
		}

		[TestMethod]
		public void GetStorageAccountList()
		{
			var storageAccountList = storageClient.GetStorageAccountList();
			Assert.IsNotNull(storageAccountList);
		}

		[TestMethod]
		public void CreateNewStorageAccountIfNotExists()
		{
			storageClient.CreateNewStorageAccountIfNotExists(storageAccountName);
		}

		[TestMethod]
		public void TryCreateNewStorageAccountIfNotExists()
		{
			Assert.IsFalse(storageClient.TryCreateNewStorageAccount(storageAccountName));
		}
	}
}