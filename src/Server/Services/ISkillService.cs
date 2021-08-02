using Simucraft.Server.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Simucraft.Server.Services
{
    public interface ISkillService
    {
        Task<SkillResponse> AddAsync(Guid userId, Guid rulesetId, SkillRequest request);

        Task DeleteAsync(Guid userId, Guid entityId);

        Task<SkillResponse> GetByIdAsync(Guid userId, Guid entityId);

        Task<IEnumerable<SkillResponse>> GetByRulesetIdAsync(Guid userId, Guid rulesetId);

        Task<SkillResponse> UpdateAsync(Guid userId, Guid rulesetId, Guid entityId, SkillRequest request);
    }
}
