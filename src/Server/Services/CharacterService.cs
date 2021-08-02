using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Simucraft.Server.Common;
using Simucraft.Server.Core;
using Simucraft.Server.DataAccess;
using Simucraft.Server.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Simucraft.Server.Services
{
    public class CharacterService : ICharacterService
    {
        private static readonly IList<string> SupportedMediaTypes = new List<string>
        {
            ".PNG",
            ".JPEG",
            ".JPG",
        };

        private readonly SimucraftContext _simucraftContext;
        private readonly IMapper _mapper;
        private readonly IRulesetBlobStorage _rulesetBlobStorage;

        public CharacterService(
            SimucraftContext simucraftContext,
            IMapper mapper,
            IRulesetBlobStorage rulesetBlobStorage)
        {
            _simucraftContext = simucraftContext;
            _mapper = mapper;
            _rulesetBlobStorage = rulesetBlobStorage;
        }

        public async Task<CharacterResponse> AddAsync(Guid userId, Guid rulesetId, CharacterRequest characterRequest)
        {
            var ruleset = await _simucraftContext.Rulesets
                .SingleOrDefaultAsync(r =>
                    r.Id == rulesetId &&
                    r.UserId == userId);

            if (ruleset == null)
                throw new NullReferenceException("Ruleset not found.");

            // TODO: I am storing small images in the document, determine if this needs to be moved to IBlobStorage.
            var character = _mapper.Map<Character>(characterRequest);
            character.Id = Guid.NewGuid();
            character.UserId = userId;
            character.RulesetId = rulesetId;

            _simucraftContext.Characters.Add(character);
            await _simucraftContext.SaveChangesAsync();

            var response = _mapper.Map<CharacterResponse>(character);
            return response;
        }

        public async Task DeleteAsync(Guid userId, Guid characterId)
        {
            var existingCharacter = await _simucraftContext.Characters
                .SingleOrDefaultAsync(c =>
                    c.Id == characterId &&
                    c.UserId == userId);

            if (existingCharacter == null)
                throw new NullReferenceException("Character not found.");

            _simucraftContext.Remove(existingCharacter);

            if (!string.IsNullOrEmpty(existingCharacter.ImageUrl))
                await _rulesetBlobStorage.DeleteAssetAsync(existingCharacter.RulesetId, existingCharacter.Id);

            await _simucraftContext.SaveChangesAsync();
        }

        public async Task<CharacterResponse> GetByIdAsync(Guid userId, Guid characterId)
        {
            var character = await _simucraftContext.Characters
                .SingleOrDefaultAsync(c =>
                    c.Id == characterId &&
                    c.UserId == userId);

            if (character == null)
                return null;

            var response = _mapper.Map<CharacterResponse>(character);
            return response;
        }

        public async Task<IEnumerable<CharacterResponse>> GetByRulesetIdAsync(Guid userId, Guid rulesetId)
        {
            var characters = await _simucraftContext.Characters
                .Where(c => c.UserId == userId &&
                            c.RulesetId == rulesetId)
                .ToListAsync();

            var responses = _mapper.Map<IEnumerable<CharacterResponse>>(characters);
            return responses;
        }

        public async Task<CharacterResponse> UpdateAsync(Guid userId, Guid rulesetId, Guid characterId, CharacterRequest characterRequest)
        {
            var character = _mapper.Map<Character>(characterRequest);
            character.Id = characterId;
            character.RulesetId = rulesetId;
            character.UserId = userId;

            var existingCharacter = await _simucraftContext.Characters
                .SingleOrDefaultAsync(c =>
                    c.Id == characterId &&
                    c.UserId == userId);

            if (existingCharacter == null)
                throw new NullReferenceException("Character not found.");

            character.ImageUrl = existingCharacter.ImageUrl;

            _simucraftContext.Characters.Remove(existingCharacter);
            _simucraftContext.Characters.Add(character);

            await _simucraftContext.SaveChangesAsync();

            var response = _mapper.Map<CharacterResponse>(character);
            return response;
        }

        public async Task<string> SetImageAsync(Guid userId, Guid characterId, IFormFile formFile)
        {
            if (formFile.Length > ByteSize.FromMegaBytes(1))
                throw new InvalidOperationException("Image cannot exceed 1 MB.");

            var fileType = Path.GetExtension(formFile.FileName);
            if (!SupportedMediaTypes.Contains(fileType.ToUpper()))
                throw new NotSupportedException("Image type not supported.");

            var character = await _simucraftContext.Characters
                .SingleOrDefaultAsync(c => c.Id == characterId &&
                                           c.UserId == userId);

            if (character == null)
                throw new NullReferenceException("Character not found.");

            using (var stream = formFile.OpenReadStream())
            { 
                var url = await _rulesetBlobStorage.SaveImageAsync(character.RulesetId, character.Id, stream);
                character.ImageUrl = url;
                character.ImageName = Path.GetFileName(formFile.FileName);
            }

            await _simucraftContext.SaveChangesAsync();

            return character.ImageUrl;
        }

        public async Task<string> CopyImageAsync(Guid userId, Guid sourceCharacterId, Guid targetCharacterId)
        {
            var sourceCharacter = await _simucraftContext.Characters
                .SingleOrDefaultAsync(c => c.Id == sourceCharacterId &&
                                           c.UserId == userId);
            var targetCharacter = await _simucraftContext.Characters
                .SingleOrDefaultAsync(c => c.Id == targetCharacterId &&
                                           c.UserId == userId);

            if (sourceCharacter == null || targetCharacter == null)
                throw new NullReferenceException("Character not found.");

            using (var stream = await _rulesetBlobStorage.GetBlobAsync(sourceCharacter.RulesetId, sourceCharacter.Id))
            {
                var url = await _rulesetBlobStorage.SaveImageAsync(targetCharacter.RulesetId, targetCharacter.Id, stream);
                targetCharacter.ImageUrl = url;
            }

            await _simucraftContext.SaveChangesAsync();

            return targetCharacter.ImageUrl;
        }
    }
}
