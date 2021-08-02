using Simucraft.Client.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Simucraft.Client.Services
{
    public interface IMapService
    {
        Task<Map> GetByIdAsync(Guid id);
        Task<IEnumerable<Map>> GetAllAsync();
        Task<IEnumerable<Map>> GetAllByRulesetIdAsync(Guid rulesetId);

        Task<Map> AddAsync(Guid rulesetId, Map map);

        Task UpdateAsync(Guid rulesetId, Map map);

        Task DeleteAsync(Guid mapId);
        Task<string> SetImageAsync(Guid mapId, string fileName, byte[] image);
    }
}
