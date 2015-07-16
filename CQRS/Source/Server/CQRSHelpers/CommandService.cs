using System;
using Microsoft.Practices.Unity;

namespace Server
{
    public class CommandService : ICommandService
    {
        private readonly UnityContainer _container;

        public CommandService(UnityContainer container)
        {
            _container = container;
        }

        public void Execute(ICommand command)
        {
            Type handlerType = typeof(ICommandHandler<>).MakeGenericType(command.GetType());
            object handler = _container.Resolve(handlerType);
            ((dynamic)handler).Handle((dynamic)command);
        }
    }
}