using Simucraft.Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Simucraft.Client.Services
{
    public interface ISkillService
    {
        Task<Skill> GetByIdAsync(Guid id);

        Task<IEnumerable<Skill>> GetAllByRulesetIdAsync(Guid rulesetId);

        Task<Skill> AddAsync(Guid rulesetId, Skill skill);

        Task<Skill> UpdateAsync(Guid rulesetId, Skill skill);

        Task DeleteAsync(Guid id);
    }
}
