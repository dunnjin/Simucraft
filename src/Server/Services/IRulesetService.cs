using Simucraft.Server.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Simucraft.Server.Services
{
    public interface IRulesetService
    {
        Task<IEnumerable<RulesetResponse>> GetByUserIdAsync(Guid userId);

        Task<RulesetResponse> GetByIdAsync(Guid userId, Guid rulesetId);

        Task<RulesetResponse> AddAsync(Guid userId, RulesetRequest rulesetRequest);
        Task<RulesetResponse> ImportAsync(Guid userId, Guid templateId);

        Task<RulesetResponse> UpdateAsync(Guid userId, Guid rulesetId, RulesetRequest rulesetRequest);

        Task DeleteAsync(Guid userId, Guid rulesetId);
    }
}
