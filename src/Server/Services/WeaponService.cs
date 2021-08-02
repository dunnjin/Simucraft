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
    public class WeaponService : IWeaponService
    {
        private readonly SimucraftContext _simucraftContext;
        private readonly IMapper _mapper;

        public WeaponService(
            SimucraftContext simucraftContext,
            IMapper mapper)
        {
            _simucraftContext = simucraftContext;
            _mapper = mapper;
        }

        public async Task<WeaponResponse> AddAsync(Guid userId, Guid rulesetId, WeaponRequest weaponRequest)
        {
            var ruleset = await _simucraftContext.Rulesets
                .SingleOrDefaultAsync(r =>
                    r.Id == rulesetId &&
                    r.UserId == userId);

            if (ruleset == null)
                throw new NullReferenceException("Ruleset not found.");

            var entity = _mapper.Map<Weapon>(weaponRequest);
            entity.Id = Guid.NewGuid();
            entity.UserId = userId;
            entity.RulesetId = rulesetId;

            _simucraftContext.Weapons.Add(entity);
            await _simucraftContext.SaveChangesAsync();

            var response = _mapper.Map<WeaponResponse>(entity);
            return response;
        }

        public async Task DeleteAsync(Guid userId, Guid weaponId)
        {
            var existingEntity = await _simucraftContext.Weapons
                .SingleOrDefaultAsync(c =>
                    c.Id == weaponId &&
                    c.UserId == userId);

            if (existingEntity == null)
                throw new NullReferenceException("Weapon not found.");

            _simucraftContext.Remove(existingEntity);
            await _simucraftContext.SaveChangesAsync();
        }

        public async Task<WeaponResponse> GetByIdAsync(Guid userId, Guid weaponId)
        {
            var entity = await _simucraftContext.Weapons
                .SingleOrDefaultAsync(c =>
                    c.Id == weaponId &&
                    c.UserId == userId);

            if (entity == null)
                return null;

            var response = _mapper.Map<WeaponResponse>(entity);
            return response;
        }

        public async Task<IEnumerable<WeaponResponse>> GetByRulesetIdAsync(Guid userId, Guid rulesetId)
        {
            var entities = await _simucraftContext.Weapons
                .Where(c => c.UserId == userId &&
                            c.RulesetId == rulesetId)
                .ToListAsync();

            var responses = _mapper.Map<IEnumerable<WeaponResponse>>(entities);
            return responses;
        }

        public async Task<WeaponResponse> UpdateAsync(Guid userId, Guid rulesetId, Guid weaponId, WeaponRequest request)
        {
            // TODO: I am storing small images in the document, determine if this needs to be moved to IBlobStorage.
            var entity = _mapper.Map<Weapon>(request);
            entity.Id = weaponId;
            entity.RulesetId = rulesetId;
            entity.UserId = userId;

            var existingEntity = await _simucraftContext.Weapons
                .SingleOrDefaultAsync(c =>
                    c.Id == weaponId &&
                    c.UserId == userId);

            if (existingEntity == null)
                throw new NullReferenceException("Weapon not found.");

            _simucraftContext.Weapons.Remove(existingEntity);
            _simucraftContext.Weapons.Add(entity);

            await _simucraftContext.SaveChangesAsync();

            var response = _mapper.Map<WeaponResponse>(entity);
            return response;
        }
    }
}
