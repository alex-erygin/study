using System;
using System.Linq;
using System.Reflection;
using Microsoft.Practices.Unity;
using Nelibur.Sword.Extensions;

namespace Server
{
    public class ServiceBuilder
    {
        private readonly UnityContainer _container;

        public ServiceBuilder(UnityContainer container)
        {
            if (container == null) throw new ArgumentNullException("container");
            _container = container;
        }

        public void RegisterDependencies()
        {
            _container.RegisterType<IQueryService, QueryService>();
            _container.RegisterType<ICommandService, CommandService>();

            Assembly.GetExecutingAssembly()
                .GetExportedTypes()
                .Where(x => (x.BaseType != null &&
                        (x.BaseType == typeof (ICommand)
                        || x.BaseType.BaseType == typeof(IQuery) 
                        || x.BaseType.BaseType == typeof (IQueryHandler) 
                        || x.BaseType.BaseType == typeof (ICommandHandler))))
                .Iter(x => _container.RegisterType(x));
        }
    }
}