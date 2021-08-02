using Simucraft.Server.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Simucraft.Server.Services
{
    public interface ISpellService
    {
        Task<SpellResponse> AddAsync(Guid userId, Guid rulesetId, SpellRequest request);

        Task DeleteAsync(Guid userId, Guid skillId);

        Task<SpellResponse> GetByIdAsync(Guid userId, Guid skillId);

        Task<IEnumerable<SpellResponse>> GetByRulesetIdAsync(Guid userId, Guid rulesetId);

        Task<SpellResponse> UpdateAsync(Guid userId, Guid rulesetId, Guid skillId, SpellRequest request);
    }
}
