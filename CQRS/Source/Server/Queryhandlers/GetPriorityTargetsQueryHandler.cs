using System.Collections.Generic;
using System.Linq;
using Contracts;
using Server.Queries;

namespace Server.Queryhandlers
{
    public class GetPriorityTargetsQueryHandler : IQueryHandler<GetPriorityTargetsQuery, List<TargetToDestroy>>
    {
        public List<TargetToDestroy> Handle(GetPriorityTargetsQuery query)
        {
            return Enumerable.Range(0, 3)
                .Select(x => new TargetToDestroy { Name = "Target #" + x }).ToList();
        }
    }
}