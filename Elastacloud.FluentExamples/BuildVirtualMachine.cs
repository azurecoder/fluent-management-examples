using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
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

        public BuildVirtualMachine(string subscriptionId, X509Certificate2 certificate)
        {
            _certificate = certificate;
            _subscriptionId = subscriptionId;
            _rdpFile = Settings.RemoteDesktopFilePath;
            _properties = new WindowsVirtualMachineProperties()
                             {
                                 AdministratorPassword = Settings.Password,
                                 RoleName = Settings.VmRoleAndServiceName,
                                 DeploymentName = Settings.VmRoleAndServiceName,
                                 Certificate = _certificate,
                                 Location = LocationConstants.NorthEurope,
                                 UseExistingCloudService = false,
                                 SubscriptionId = subscriptionId,
                                 CloudServiceName = Settings.VmRoleAndServiceName,
                                 PublicEndpoints = new List<InputEndpoint>(new[]
                                                                               {
                                                                                   new InputEndpoint()
                                                                                       {
                                                                                           EndpointName = "web",
                                                                                           LocalPort = 80,
                                                                                           Port = 80,
                                                                                           Protocol = Protocol.TCP
                                                                                       }
                                                                               }),
                                 VirtualMachineType = VirtualMachineTemplates.WindowsServer2012,
                                 VmSize = VmSize.Medium,
                                 StorageAccountName = Settings.DefaultStorage,
                                 DataDisks = new List<DataVirtualHardDisk>(new[] {
                                     new DataVirtualHardDisk(){LogicalDiskSizeInGB = 100}
                                 })
                             };
        }

        void IBuilder.SpinUp()
        {
            var storageClient = new StorageClient(_subscriptionId, _certificate);
            storageClient.CreateNewStorageAccount(Settings.DefaultStorage);
            var client = new WindowsVirtualMachineClient(_subscriptionId, _certificate);
            var newClient = client.CreateNewVirtualMachineFromTemplateGallery(_properties);
            Console.WriteLine("Virtual machine now created - with diskname {0}", 
                newClient.VirtualMachine.OSHardDisk.DiskName);
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

        public void DoSomething()
        {
            Console.WriteLine("Nothing to do!");
        }

        X509Certificate2 IBuilder.ManagementCertificate
        {
            get { return _certificate; }
        }

        string IBuilder.SubscriptionId
        {
            get { return _subscriptionId; }
        }

        public override string ToString()
        {
            return "BuildVirtualMachine";
        }
    }
}




