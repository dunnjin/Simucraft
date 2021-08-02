using Simucraft.Server.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Simucraft.Server.Services
{
    public interface IEquipmentService
    {
        Task<EquipmentResponse> AddAsync(Guid userId, Guid rulesetId, EquipmentRequest request);

        Task DeleteAsync(Guid userId, Guid entityId);

        Task<EquipmentResponse> GetByIdAsync(Guid userId, Guid entityId);

        Task<IEnumerable<EquipmentResponse>> GetByRulesetIdAsync(Guid userId, Guid rulesetId);

        Task<EquipmentResponse> UpdateAsync(Guid userId, Guid rulesetId, Guid weaponId, EquipmentRequest request);
    }
}
