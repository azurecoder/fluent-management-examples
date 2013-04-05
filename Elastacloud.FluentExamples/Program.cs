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
            // create a virtual machine
            IBuilder virtualMachine = new BuildVirtualMachine(args[0], args[1], args[2]);

            // test linq to azure with storage
            IWorkflow linqToStorage = new LinqToStorage(virtualMachine.SubscriptionId, virtualMachine.ManagementCertificate);
            linqToStorage.PreActionSteps();
            linqToStorage.DoWork();
            linqToStorage.PostActionSteps();
            Debugger.Break();

            // test linq to azure with cloud services

            // test role system watcher

            // test paas deployment

            // test paas orchestration

            // test create a mobile services deployment

            // manipulate and transform config files

            // test virtual machine deployment
            virtualMachine.SpinUp();
            Debugger.Break();
            virtualMachine.TearDown();

            Console.WriteLine("Press [ENTER] to exit");
            Console.Read();
        }
    }
}
