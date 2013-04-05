using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Elastacloud.AzureManagement.Fluent.Clients;
using Elastacloud.AzureManagement.Fluent.Commands.VirtualMachines;
using Elastacloud.AzureManagement.Fluent.Helpers;
using Elastacloud.AzureManagement.Fluent.Helpers.PublishSettings;
using Elastacloud.AzureManagement.Fluent.Types;
using Elastacloud.AzureManagement.Fluent.Types.VirtualMachines;
using Elastacloud.AzureManagement.Fluent.VirtualMachines.Classes;

namespace Elastacloud.FluentExamples
{
	public class BuildVirtualMachine : IBuilder
	{
		private readonly WindowsVirtualMachineProperties _properties;
		private readonly X509Certificate2 _certificate;
		private readonly string _subscriptionId;
		private readonly string _rdpFile;
		private readonly string _storageAccountName;
		private readonly string _storageLocationName;

		public BuildVirtualMachine(string subscriptionId, string publishSettingsFile, string rdpFile, string storageAccountName, string storageLocationName, string cloudServiceName, string deploymentName, string roleName, string administratorPassWord)
		{
			var settings = PublishSettingsExtractor.GetFromFile(publishSettingsFile);
			_certificate = settings.AddPublishSettingsToPersonalMachineStore();
			_subscriptionId = subscriptionId;
			_rdpFile = rdpFile;
			_storageAccountName = storageAccountName;
			_storageLocationName = storageLocationName;
			_properties = new WindowsVirtualMachineProperties
											{
												 AdministratorPassword = administratorPassWord,
												 RoleName = roleName,
												 DeploymentName = deploymentName,
												 Certificate = _certificate,
												 Location = LocationConstants.NorthEurope,
												 UseExistingCloudService = false,
												 SubscriptionId = subscriptionId,
												 CloudServiceName = cloudServiceName,
												 PublicEndpoints = new List<InputEndpoint>(new[]
												 {
													 new InputEndpoint
													 {
														 EndpointName = "web",
														 LocalPort = 80,
														 Port = 80,
														 Protocol = Protocol.TCP
													 }
												 }),
												 VirtualMachineType = VirtualMachineTemplates.WindowsServer2012,
												 VmSize = VmSize.Medium,
												 StorageAccountName = _storageAccountName,
												 DataDisks = new List<DataVirtualHardDisk>(new[]
												 {
													 new DataVirtualHardDisk
														 {
															 LogicalDiskSizeInGB = 100
														 }
												 })
											 };
		}

		void IBuilder.SpinUp()
		{
			var storageClient = new StorageClient(_subscriptionId, _certificate);
			storageClient.CreateNewStorageAccountIfNotExists(_storageAccountName, _storageLocationName);
			var client = new WindowsVirtualMachineClient(_subscriptionId, _certificate);
			var newClient = client.CreateNewVirtualMachineFromTemplateGallery(_properties);
			Console.WriteLine("Virtual machine now created - with diskname {0}", newClient.VirtualMachine.OSHardDisk.DiskName);
			Console.WriteLine("Getting and saving RD file");
			client.SaveRemoteDesktopFile(_rdpFile);
		}

		void IBuilder.TearDown()
		{
			var client = new WindowsVirtualMachineClient(_properties);
			string ipAddress = client.VirtualMachine.NetworkConfigurationSet.InputEndpoints[0].Vip;

			Console.WriteLine("The VIP is {0}", ipAddress);

			client.DeleteVirtualMachine();
			Console.WriteLine("Virtual machine has been deleted, with cloud service and storage");
		}

		X509Certificate2 IBuilder.ManagementCertificate
		{
			get { return _certificate; }
		}

		string IBuilder.SubscriptionId
		{
			get { return _subscriptionId; }
		}
	}
}