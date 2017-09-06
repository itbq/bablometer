using Autofac;
using Autofac.Integration.Mvc;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Plugin.ExternalAuth.Vkontakte.Core;

namespace Nop.Plugin.ExternalAuth.Vkontakte
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            builder.RegisterType<VkontakteProviderAuthorizer>().As<IOAuthProviderVkontakteAuthorizer>().InstancePerHttpRequest();
        }

        public int Order
        {
            get { return 1; }
        }
    }
}
