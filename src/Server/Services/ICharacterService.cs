using Microsoft.AspNetCore.Http;
using Simucraft.Server.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Simucraft.Server.Services
{
    public interface ICharacterService
    {
        Task<IEnumerable<CharacterResponse>> GetByRulesetIdAsync(Guid userId, Guid rulesetId);

        Task<CharacterResponse> GetByIdAsync(Guid userId, Guid characterId);

        Task<CharacterResponse> AddAsync(Guid userId, Guid rulesetId, CharacterRequest characterRequest);

        Task<CharacterResponse> UpdateAsync(Guid userId, Guid rulesetId, Guid characterId, CharacterRequest characterRequest);

        Task DeleteAsync(Guid userId, Guid characterId);
        Task<string> SetImageAsync(Guid userId, Guid characterId, IFormFile formFile);
        Task<string> CopyImageAsync(Guid userId, Guid sourceCharacterId, Guid targetCharacterId);
    }
}
