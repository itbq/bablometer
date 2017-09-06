using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace BindingServiceLib
{
    public class IISConfigurerWindowsSerivce : ServiceBase
    {
        public ServiceHost _serviceHost = null;

        public IISConfigurerWindowsSerivce()
        {
            ServiceName = "ETFIISConfigurer";
        }

        public static void Main()
        {
            ServiceBase.Run(new IISConfigurerWindowsSerivce());
        }

        protected override void OnStart(string[] args)
        {
            if (_serviceHost != null)
            {
                _serviceHost.Close();
            }

            _serviceHost = new ServiceHost(typeof(IISConfigurer));
            _serviceHost.Open();
        }

        protected override void OnStop()
        {
            if (_serviceHost != null)
            {
                _serviceHost.Close();
                _serviceHost = null;
            }
        }
    }
}
