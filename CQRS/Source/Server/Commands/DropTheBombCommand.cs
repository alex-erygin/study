using System;
using Contracts;

namespace Server.Commands
{
    public class DropTheBombCommand : ICommand
    {
        private readonly TargetToDestroy _target;

        public DropTheBombCommand(TargetToDestroy target)
        {
            if (target == null) throw new ArgumentNullException("target");
            _target = target;
        }

        public TargetToDestroy Target
        {
            get { return _target; }
        }
    }
}