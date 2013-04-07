using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Elastacloud.AzureManagement.Fluent.Clients;

namespace Elastacloud.FluentExamples
{
    public class BuildGetBlobRequest : IWorkflow 
    {
        private readonly string _subscriptionId;
        private readonly X509Certificate2 _managementCertificate;

        public BuildGetBlobRequest(string subscriptionId, X509Certificate2 managementCertificate)
        {
            _subscriptionId = subscriptionId;
            _managementCertificate = managementCertificate;
        }

        #region Implementation of IWorkflow

        public void PreActionSteps()
        {
            // use AMS to transfer file
            var storageClient = new StorageClient(_subscriptionId, _managementCertificate);
            var keys = storageClient.GetStorageAccountKeys(Settings.DefaultStorage);
            StorageKey = keys[0];
        }

        public void PostActionSteps()
        {
            Console.WriteLine("no postbuild action steps");
        }

        public void DoWork()
        {
            var date = DateTime.UtcNow.ToString("R");
            var lineEndings = new string('\n', 12);
            var builder = new StringBuilder("GET");
            builder.Append(lineEndings);
            builder.AppendFormat("x-ms-date:{0}\n", date);
            builder.Append("x-ms-version:2011-08-18\n");
            builder.AppendFormat("/{0}/docs/test.txt\n",Settings.DefaultStorage);
            builder.Append("timeout:90");

            string signature;
            using (var hmacSha256 = new HMACSHA256(Convert.FromBase64String(StorageKey)))
            {
                Byte[] bytes = Encoding.UTF8.GetBytes(builder.ToString());
                signature = Convert.ToBase64String(hmacSha256.ComputeHash(bytes));
            }

            var authValue = "SharedKey stackedstorage:" + signature;

            var request = (HttpWebRequest)WebRequest.Create(
                String.Format("http://{0}.blob.core.windows.net/docs/test.txt?timeout=90", Settings.DefaultStorage));
            request.Method = "GET";
            request.ContentLength = 0;
            request.Headers.Add("x-ms-date", date);
            request.Headers.Add("x-ms-version", "2011-08-18");
            request.Headers.Add("Authorization", authValue);
            request.GetResponse();

            Console.WriteLine("Getting blob now!");
        }

        #endregion

        public string StorageKey { get; private set; }

        public override string ToString()
        {
            return "BuildGetBlobRequest";
        }
    }
}
