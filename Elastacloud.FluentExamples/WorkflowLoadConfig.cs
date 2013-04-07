using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Elastacloud.AzureManagement.Fluent.Clients;
using Elastacloud.AzureManagement.Fluent.Helpers;
using Elastacloud.AzureManagement.Fluent.Helpers.PublishSettings;
using Elastacloud.AzureManagement.Fluent.Linq;
using Elastacloud.AzureManagement.Fluent.Types;

namespace Elastacloud.FluentExamples
{
    public class WorkflowLoadConfig : IWorkflow
    {
        private readonly string _configFile;
        private List<string> roleList;
        private int instanceCount;
        private CscfgFile configFile;

        public WorkflowLoadConfig(string configFile)
        {
            _configFile = configFile;
        }

        #region Implementation of IWorkflow

        public void PreActionSteps()
        {
            Console.WriteLine("getting configuration settings for role");
            configFile = (CscfgFile)CscfgFile.GetInstance(_configFile);
            roleList = configFile.GetRoleNameList();
        }

        public void DoWork()
        {
            foreach (var role in roleList)
                Console.WriteLine("Found role with name: {0}, has {1} instances", role, configFile.GetInstanceCountForRole(role));
        }

        public void PostActionSteps()
        {
            Console.WriteLine("Finished reading .cscfg file");
        }

        #endregion

        public override string ToString()
        {
            return "WorkflowLoadConfig";
        }
    }
}
