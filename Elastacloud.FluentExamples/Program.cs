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
        static readonly Dictionary<string, IBuilder> Builders = new Dictionary<string, IBuilder>();
        static readonly Dictionary<string, IWorkflow> Workflows = new Dictionary<string, IWorkflow>();
        // args 0 - the subscription id
        static void Main(string[] args)
        {
            if (args.Length < 1)
                throw new ApplicationException("need a single argument to proceed");

            // manipulate and transform config files
            IWorkflow getConfig = new WorkflowLoadConfig(Path.Combine(Settings.DeploymentPath, "ServiceConfiguration.Cloud.cscfg"));
            Workflows.Add(getConfig.ToString(), getConfig);
            ProcessWorkflow(getConfig.ToString());

            // do the publishsettings 
            IWorkflow getSettings = new WorkflowPublishSettings(args[0], Settings.PublishSettingsFilePath);
            Workflows.Add(getSettings.ToString(), getSettings);
            ProcessWorkflow(getSettings.ToString());

            // Add the test get blob API - before running this create a storage account using AMS called stackedstorage
            IWorkflow getBlob = new BuildGetBlobRequest(Settings.SubscriptionId, Settings.ManagementCertificate);
            Workflows.Add(getBlob.ToString(), getBlob);
            ProcessWorkflow(getBlob.ToString());

            // test paas deployment
            // test linq to azure with cloud services
            // test paas orchestration
            IWorkflow fluentDeployment = new WorkflowFluentDeployment(Settings.SubscriptionId, Settings.ManagementCertificate);
            Workflows.Add(fluentDeployment.ToString(), fluentDeployment);
            //ProcessWorkflow(fluentDeployment.ToString());

            // create a virtual machine
            IBuilder virtualMachine = new BuildVirtualMachine(args[0], Settings.ManagementCertificate);
            Builders.Add(virtualMachine.ToString(), virtualMachine);
            //ProcessBuilder(virtualMachine.ToString());

            // test linq to azure with storage
            IWorkflow linqToStorage = new WorkflowLinqToStorage(Settings.SubscriptionId, Settings.ManagementCertificate);
            Workflows.Add(linqToStorage.ToString(), linqToStorage);
            //ProcessWorkflow(linqToStorage.ToString());

            // test create a mobile services deployment
            IBuilder mobileService = new BuildMobileService(Settings.SubscriptionId, Settings.ManagementCertificate);
            Builders.Add(mobileService.ToString(), mobileService);
            //ProcessBuilder(mobileService.ToString());
            
            // test role system watcher
            var watcher = new WorkflowRoleSystemWatcher(Settings.SubscriptionId, Settings.ManagementCertificate);
            Workflows.Add(watcher.ToString(), watcher);
            //ProcessWorkflow(watcher.ToString());

            // test paas build
            var workflowSSL = new WorkflowFluentDeploymentWithSSL(Settings.SubscriptionId, Settings.ManagementCertificate);
            Workflows.Add(workflowSSL.ToString(), workflowSSL);
            //ProcessWorkflow(workflowSSL.ToString());
            
            Console.WriteLine("Press [ENTER] to exit");
            Console.Read();
        }

        static void ProcessWorkflow(string workflowName)
        {
            var workflow = Workflows[workflowName];
            workflow.PreActionSteps();
            workflow.DoWork();
            workflow.PostActionSteps();
        }

        static void ProcessBuilder(string builderName)
        {
            var builder = Builders[builderName];
            builder.SpinUp();
            builder.DoSomething();
            builder.TearDown();
        }
    }
}
