using System.Security.Cryptography.X509Certificates;

namespace Elastacloud.FluentExamples
{
    interface IBuilder
    {
        void SpinUp();
        void TearDown();
        string SubscriptionId { get; }
        X509Certificate2 ManagementCertificate { get; }
    }
}