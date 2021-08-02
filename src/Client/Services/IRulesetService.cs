using Simucraft.Client.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Simucraft.Client.Services
{
    public interface IRulesetService
    {
        Task<Ruleset> GetByIdAsync(Guid id);

        Task<IEnumerable<Ruleset>> GetAllAsync();

        Task<Ruleset> AddAsync(Ruleset ruleset);

        Task UpdateAsync(Ruleset ruleset);

        Task DeleteAsync(Guid id);

        Task<Ruleset> ImportAsync(Guid templateId);
    }
}
