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

		/// <summary>
		/// args 0 - the subscription id
		/// args 1 - the path to the publishsettings file
		/// args 2 - the path to the rdp output file 
		/// args 3 - the storage account name where the VM VHDs will be stored
		/// args 3 - the storage account location name 'North Europe'
		/// args 4 - the Cloud Service name
		/// args 5 - the deployment name
		/// args 6 - the role name
		/// args 7 - the administrator password for the VM
		/// </summary>
		/// <param name="args"></param>
		static void Main(string[] args)
		{
			var subscriptionId = args[0];
			var publishSettingsFile = args[1];
			var rdpFile = args[2];
			var storageAccountName = args[3];
			var storageLocationName = args[4];
			var cloudServiceName = args[5];
			var deploymentName = args[6];
			var roleName = args[7];
			var administratorPassWord = args[8];

			IBuilder virtualMachine = new BuildVirtualMachine(subscriptionId, publishSettingsFile, rdpFile, storageAccountName, storageLocationName, cloudServiceName, deploymentName, roleName, administratorPassWord);
			virtualMachine.SpinUp();
			Debugger.Break();
			virtualMachine.TearDown();

			Console.WriteLine("Press [ENTER] to exit");
			Console.Read();
		}
	}
}
