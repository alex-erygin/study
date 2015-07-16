using System.Collections.Generic;
using System.Linq;
using Contracts;
using Server.Queries;

namespace Server.Queryhandlers
{
    public class GetAllTargetsQueryHandler : IQueryHandler<GetAllTargetsQuery, List<TargetToDestroy>> 
    {
        public List<TargetToDestroy> Handle(GetAllTargetsQuery query)
        {
            return Enumerable.Range(0, 100)
                .Select(x => new TargetToDestroy {Name = "Target #" + x}).ToList();
        }
    }
}