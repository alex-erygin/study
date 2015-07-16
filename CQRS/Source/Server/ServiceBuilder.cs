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
            _container.RegisterInstance(_container, new ContainerControlledLifetimeManager());
            _container.RegisterType<IQueryService, QueryService>();
            _container.RegisterType<ICommandService, CommandService>();

            var queries = Assembly.GetExecutingAssembly()
                .GetExportedTypes()
                .Where(x => x.Name.EndsWith("Query") && !x.IsAbstract)
                .ToList();

            var commands = Assembly.GetExecutingAssembly()
                .GetExportedTypes()
                .Where(x => x.Name.EndsWith("Command") && !x.IsAbstract)
                .ToList();

            queries.Union(commands)
                .Union(commands)
                .Iter(x => _container.RegisterType(x));

            var queryHandlers = Assembly.GetExecutingAssembly()
                .GetExportedTypes()
                .Where(x => x.Name.EndsWith("QueryHandler") && !x.IsAbstract).ToList();


                queryHandlers.Iter(x=>_container.RegisterType(x.GetInterfaces()[0], x));

            Assembly.GetExecutingAssembly()
                .GetExportedTypes()
                .Where(x => x.Name.EndsWith("CommandHandler") && !x.IsAbstract)
                .Iter(x => _container.RegisterType(x.GetInterfaces()[0], x));
        }
    }
}