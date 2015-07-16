using System;
using Microsoft.Practices.Unity;

namespace Server
{
    public class QueryService : IQueryService
    {
        private readonly UnityContainer _container;

        public QueryService(UnityContainer container)
        {
            _container = container;
        }

        public TResult Query<TResult>(IQuery<TResult> query)
        {
            Type handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));
            object handler = _container.Resolve(handlerType);
            return (TResult)((dynamic)handler).Handle((dynamic)query);
        }
    }
}