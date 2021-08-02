using Simucraft.Client.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Simucraft.Client.Services
{
    public interface IWeaponService
    {
        Task<Weapon> GetByIdAsync(Guid id);

        Task<IEnumerable<Weapon>> GetAllByRulesetIdAsync(Guid rulesetId);

        Task<Weapon> AddAsync(Guid rulesetId, Weapon weapon);

        Task<Weapon> UpdateAsync(Guid rulesetId, Weapon weapon);

        Task DeleteAsync(Guid id);
    }
}
