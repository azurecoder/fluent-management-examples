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
            IBuilder virtualMachine = new BuildVirtualMachine(args[0], args[1], args[2]);
            virtualMachine.SpinUp();
            Debugger.Break();
            virtualMachine.TearDown();

            Console.WriteLine("Press [ENTER] to exit");
            Console.Read();
        }
    }
}
