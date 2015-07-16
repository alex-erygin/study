using System;
using Contracts.Messages;
using Nelibur.ServiceModel.Services.Operations;
using Server.Queries;

namespace Server.MessageHandlers
{
    public class GetPriorityTargetsMessageHandler : IGet<GetPriorityTargetsMessage>
    {
        private readonly IQueryService _queryService;

        public GetPriorityTargetsMessageHandler(IQueryService queryService)
        {
            if (queryService == null) throw new ArgumentNullException("queryService");
            _queryService = queryService;
        }

        public object Get(GetPriorityTargetsMessage request)
        {
            return _queryService.Query(new GetPriorityTargetsQuery());
        }
    }
}