using Simucraft.Server.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Simucraft.Server.Services
{
    public interface IWeaponService
    {
        Task<WeaponResponse> AddAsync(Guid userId, Guid rulesetId, WeaponRequest weaponRequest);

        Task DeleteAsync(Guid userId, Guid weaponId);

        Task<WeaponResponse> GetByIdAsync(Guid userId, Guid weaponId);

        Task<IEnumerable<WeaponResponse>> GetByRulesetIdAsync(Guid userId, Guid rulesetId);

        Task<WeaponResponse> UpdateAsync(Guid userId, Guid rulesetId, Guid weaponId, WeaponRequest request);
    }
}
