using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Elastacloud.FluentExamples
{
    class Program
    {
        // args 0 - the subscription id
        // args 1 - the path to the publishsettings file
        // args 2 - the path to the rdp output file 
        static void Main(string[] args)
        {
            var builders = new List<IBuilder>();
            var workflows = new List<IWorkflow>();
            // create a virtual machine
            IBuilder virtualMachine = new BuildVirtualMachine(args[0], args[1], args[2]);
            builders.Add(virtualMachine);

            // Add the test get blob API - before running this create a storage account using AMS called stackedstorage
            IWorkflow getBlob = new BuildGetBlobRequest(virtualMachine.SubscriptionId, virtualMachine.ManagementCertificate);
            workflows.Add(getBlob);

            // test linq to azure with storage
            IWorkflow linqToStorage = new WorkflowLinqToStorage(virtualMachine.SubscriptionId, virtualMachine.ManagementCertificate);
            workflows.Add(linqToStorage);

            // test create a mobile services deployment
            IBuilder mobileService = new BuildMobileService(virtualMachine.SubscriptionId, virtualMachine.ManagementCertificate);
            builders.Add(mobileService);

            // test paas deployment
            // test linq to azure with cloud services
            // test paas orchestration
            IWorkflow fluentDeployment = new WorkflowFluentDeployment(virtualMachine.SubscriptionId, virtualMachine.ManagementCertificate);
            workflows.Add(fluentDeployment);
            
            // test role system watcher
            var watcher = new WorkflowRoleSystemWatcher(virtualMachine.SubscriptionId, virtualMachine.ManagementCertificate);
            workflows.Add(watcher);
            watcher.DoWork();

            // test paas build

            


            
            // manipulate and transform config files
            Console.WriteLine("Press [ENTER] to exit");
            Console.Read();
        }
    }
}
