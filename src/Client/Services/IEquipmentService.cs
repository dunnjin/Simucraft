using Simucraft.Client.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Simucraft.Client.Services
{
    public interface IEquipmentService
    {
        Task<Equipment> GetByIdAsync(Guid id);

        Task<IEnumerable<Equipment>> GetAllByRulesetIdAsync(Guid rulesetId);

        Task<Equipment> AddAsync(Guid rulesetId, Equipment equipment);

        Task<Equipment> UpdateAsync(Guid rulesetId, Equipment equipment);

        Task DeleteAsync(Guid id);
    }
}
