using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AxTools.WoW.Internals
{
    internal enum ObjectType : byte
    {
        Object = 0,
        Item = 1,
        Container = 2,
        AzeriteEmpoweredItem = 3,
        AzeriteItem = 4,
        Unit = 5,
        Player = 6,
        ActivePlayer = 7,
        GameObject = 8,
        Dynamic = 9,
        Corpse = 10,
        Areatrigger = 11,
        Scene = 12,
        Conversation = 13,
        AiGroup = 14,
        Scenario = 15,
        Loot = 16,
        Invalid = 17
    }
}
