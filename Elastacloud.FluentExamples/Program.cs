using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Elastacloud.FluentExamples
{
    class Program
    {
        static readonly List<IBuilder> Builders = new List<IBuilder>();
        static readonly List<IWorkflow> Workflows = new List<IWorkflow>();
        // args 0 - the subscription id
        // args 1 - the path to the publishsettings file
        // args 2 - the path to the rdp output file 
        static void Main(string[] args)
        {
            if (args.Length < 1)
                throw new ApplicationException("need a single argument to proceed");

            // manipulate and transform config files
            IWorkflow getConfig = new WorkflowLoadConfig(Path.Combine(Settings.DeploymentPath, "TestCloudInstall.cscfg"));
            getConfig.PreActionSteps();

            // do the publishsettings 
            IWorkflow getSettings = new WorkflowPublishSettings(args[0], Settings.PublishSettingsFilePath);
            getSettings.PreActionSteps();

            // Add the test get blob API - before running this create a storage account using AMS called stackedstorage
            IWorkflow getBlob = new BuildGetBlobRequest(Settings.SubscriptionId, Settings.ManagementCertificate);
            Workflows.Add(getBlob);

            // create a virtual machine
            IBuilder virtualMachine = new BuildVirtualMachine(args[0], Settings.ManagementCertificate);
            Builders.Add(virtualMachine);

            // test linq to azure with storage
            IWorkflow linqToStorage = new WorkflowLinqToStorage(Settings.SubscriptionId, Settings.ManagementCertificate);
            Workflows.Add(linqToStorage);

            // test create a mobile services deployment
            IBuilder mobileService = new BuildMobileService(Settings.SubscriptionId, Settings.ManagementCertificate);
            Builders.Add(mobileService);

            // test paas deployment
            // test linq to azure with cloud services
            // test paas orchestration
            IWorkflow fluentDeployment = new WorkflowFluentDeployment(Settings.SubscriptionId, Settings.ManagementCertificate);
            Workflows.Add(fluentDeployment);
            
            // test role system watcher
            var watcher = new WorkflowRoleSystemWatcher(virtualMachine.SubscriptionId, virtualMachine.ManagementCertificate);
            Workflows.Add(watcher);

            // test paas build
            
            Console.WriteLine("Press [ENTER] to exit");
            Console.Read();
        }
    }
}
