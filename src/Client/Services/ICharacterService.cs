using Simucraft.Client.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Simucraft.Client.Services
{
    public interface ICharacterService
    {
        Task<Character> GetByIdAsync(Guid id);

        Task<IEnumerable<Character>> GetAllByRulesetIdAsync(Guid rulesetId);

        Task<Character> AddAsync(Guid rulesetId, Character character);

        Task<Character> UpdateAsync(Guid rulesetId, Character character);

        Task DeleteAsync(Guid characterId);
        Task<string> SetImageAsync(Guid characterId, string imageName, byte[] image);
        Task<string> CopyImageAsync(Guid sourceCharacterId, Guid targetCharacterId);
    }
}
