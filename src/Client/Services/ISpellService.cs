using Simucraft.Client.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Simucraft.Client.Services
{
    public interface ISpellService
    {
        Task<Spell> GetByIdAsync(Guid id);

        Task<IEnumerable<Spell>> GetAllByRulesetIdAsync(Guid rulesetId);

        Task<Spell> AddAsync(Guid rulesetId, Spell skill);

        Task<Spell> UpdateAsync(Guid rulesetId, Spell skill);

        Task DeleteAsync(Guid id);
    }
}
