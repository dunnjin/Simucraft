using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Simucraft.Server.Core;
using Simucraft.Server.DataAccess;
using Simucraft.Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Simucraft.Server.Services
{
    public class SpellService : ISpellService
    {
        private readonly SimucraftContext _simucraftContext;
        private readonly IMapper _mapper;

        public SpellService(
            SimucraftContext simucraftContext,
            IMapper mapper)
        {
            _simucraftContext = simucraftContext;
            _mapper = mapper;
        }

        public async Task<SpellResponse> AddAsync(Guid userId, Guid rulesetId, SpellRequest weaponRequest)
        {
            var ruleset = await _simucraftContext.Rulesets
                .SingleOrDefaultAsync(r =>
                    r.Id == rulesetId &&
                    r.UserId == userId);

            if (ruleset == null)
                throw new NullReferenceException("Ruleset not found.");

            var entity = _mapper.Map<Spell>(weaponRequest);
            entity.Id = Guid.NewGuid();
            entity.UserId = userId;
            entity.RulesetId = rulesetId;

            _simucraftContext.Spells.Add(entity);
            await _simucraftContext.SaveChangesAsync();

            var response = _mapper.Map<SpellResponse>(entity);
            return response;
        }

        public async Task DeleteAsync(Guid userId, Guid weaponId)
        {
            var existingEntity = await _simucraftContext.Spells
                .SingleOrDefaultAsync(c =>
                    c.Id == weaponId &&
                    c.UserId == userId);

            if (existingEntity == null)
                throw new NullReferenceException("Spell not found.");

            _simucraftContext.Remove(existingEntity);
            await _simucraftContext.SaveChangesAsync();
        }

        public async Task<SpellResponse> GetByIdAsync(Guid userId, Guid weaponId)
        {
            var entity = await _simucraftContext.Spells
                .SingleOrDefaultAsync(c =>
                    c.Id == weaponId &&
                    c.UserId == userId);

            if (entity == null)
                return null;

            var response = _mapper.Map<SpellResponse>(entity);
            return response;
        }

        public async Task<IEnumerable<SpellResponse>> GetByRulesetIdAsync(Guid userId, Guid rulesetId)
        {
            var entities = await _simucraftContext.Spells
                .Where(c => c.UserId == userId &&
                            c.RulesetId == rulesetId)
                .ToListAsync();

            var responses = _mapper.Map<IEnumerable<SpellResponse>>(entities);
            return responses;
        }

        public async Task<SpellResponse> UpdateAsync(Guid userId, Guid rulesetId, Guid weaponId, SpellRequest request)
        {
            // TODO: I am storing small images in the document, determine if this needs to be moved to IBlobStorage.
            var entity = _mapper.Map<Spell>(request);
            entity.Id = weaponId;
            entity.RulesetId = rulesetId;
            entity.UserId = userId;

            var existingEntity = await _simucraftContext.Spells
                .SingleOrDefaultAsync(c =>
                    c.Id == weaponId &&
                    c.UserId == userId);

            if (existingEntity == null)
                throw new NullReferenceException("Spell not found.");

            _simucraftContext.Spells.Remove(existingEntity);
            _simucraftContext.Spells.Add(entity);

            await _simucraftContext.SaveChangesAsync();

            var response = _mapper.Map<SpellResponse>(entity);
            return response;
        }
    }
}
