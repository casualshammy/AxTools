using System;
using System.Threading;
using System.Threading.Tasks;

namespace AxTools.Classes
{
    internal class NamedTask : Task
    {
        internal NamedTask(Action action, CancellationToken cancellationToken, TaskCreationOptions options, string name, string wowIcon) : base(action, cancellationToken, options)
        {
            Name = name;
            WowIcon = wowIcon;
        }

        internal string Name;
        internal string WowIcon;
    }
}
