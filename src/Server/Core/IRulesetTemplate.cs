using Simucraft.Server.Models;
using System;
using System.Collections.Generic;

namespace Simucraft.Server.Core
{
    public interface IRulesetTemplate
    {
        Ruleset Ruleset(Guid userId);

        IEnumerable<Weapon> Weapons(Guid userId, Guid rulesetId);

        IEnumerable<Character> Characters(Guid userId, Guid rulesetId);
    }
}
