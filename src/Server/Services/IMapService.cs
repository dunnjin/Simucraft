using Microsoft.AspNetCore.Http;
using Simucraft.Server.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Simucraft.Server.Services
{
    public interface IMapService
    {
        Task<IEnumerable<MapResponse>> GetByRulesetIdAsync(Guid userId, Guid rulesetId);
        Task<IEnumerable<MapResponse>> GetByUserIdAsync(Guid userId);

        Task<MapResponse> GetByIdAsync(Guid userId, Guid mapId);

        Task<MapResponse> AddAsync(Guid userId, Guid rulesetId, MapRequest map);

        Task<MapResponse> UpdateAsync(Guid userId, Guid rulesetId, Guid mapId, MapRequest map);

        Task DeleteAsync(Guid userId, Guid mapId);
        Task SetImageAsync(Guid userId, Guid mapId, IFormFile formFile);
    }
}
