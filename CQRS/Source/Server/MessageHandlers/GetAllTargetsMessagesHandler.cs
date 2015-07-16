using System;
using Contracts.Messages;
using Nelibur.ServiceModel.Services.Operations;
using Server.Queries;

namespace Server.MessageHandlers
{
    public class GetAllTargetsMessagesHandler : IGet<GetAllTargetsMessage>
    {
        private readonly IQueryService _queryService;

        public GetAllTargetsMessagesHandler(IQueryService queryService)
        {
            if (queryService == null) throw new ArgumentNullException("queryService");
            _queryService = queryService;
        }

        public object Get(GetAllTargetsMessage message)
        {
            return _queryService.Query(new GetAllTargetsQuery());
        }
    }
}