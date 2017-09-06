using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace BindingServiceLib
{
    [ServiceContract]
    interface IIISConfigurer
    {
        [OperationContract]
        bool AddBinding(string domainName);

        [OperationContract]
        bool DeleteBinding(string domainName);

        [OperationContract]
        string AddSubdominNSRecord(string subdomainName);

        [OperationContract]
        string DeleteSubdomain(string domainName);
    }
}
