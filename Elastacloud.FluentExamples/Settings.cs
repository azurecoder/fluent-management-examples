using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Elastacloud.FluentExamples
{
    public static class Settings
    {
        public static string DatabaseName { get { return ConfigurationManager.AppSettings["dbname"]; } }
        public static string Username { get { return ConfigurationManager.AppSettings["username"]; } }
        public static string Password { get { return ConfigurationManager.AppSettings["password"]; } }
        public static string SqlDatabaseUsername { get { return ConfigurationManager.AppSettings["sqldbusername"]; } }
        public static string DefaultStorage { get { return ConfigurationManager.AppSettings["defaultStorage"]; } }
        public static string MobileServiceName { get { return ConfigurationManager.AppSettings["mobileservicename"]; } }
        public static string DeplopymentName { get { return ConfigurationManager.AppSettings["deploymentname"]; } }
        public static string PublishSettingsFilePath { get { return ConfigurationManager.AppSettings["publishsettings"]; } }
        public static string RemoteDesktopFilePath { get { return ConfigurationManager.AppSettings["rdpfile"]; } }
        public static string VmRoleAndServiceName { get { return ConfigurationManager.AppSettings["vmservicerole"]; } }
        public static string CloudServiceName { get { return ConfigurationManager.AppSettings["cloudservice"]; } }
        public static string RoleName { get { return ConfigurationManager.AppSettings["rolename"]; } }
        public static string DeploymentPath { get { return ConfigurationManager.AppSettings["deploymentpath"]; } }
        public static string SubscriptionId { get; set; }
        public static X509Certificate2 ManagementCertificate { get; set; }
    }
}
