using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Simucraft.Server.Common;
using Simucraft.Server.Core;
using Simucraft.Server.DataAccess;
using Simucraft.Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Simucraft.Server.Services
{
    public class EquipmentService  : IEquipmentService
    {
        private readonly SimucraftContext _simucraftContext;
        private readonly IMapper _mapper;

        public EquipmentService(
            SimucraftContext simucraftContext,
            IMapper mapper)
        {
            _simucraftContext = simucraftContext;
            _mapper = mapper;
        }

        public async Task<EquipmentResponse> AddAsync(Guid userId, Guid rulesetId, EquipmentRequest request)
        {
            var ruleset = await _simucraftContext.Rulesets
                .SingleOrDefaultAsync(r =>
                    r.Id == rulesetId &&
                    r.UserId == userId);

            if (ruleset == null)
                throw new NullReferenceException("Ruleset not found.");

            var equipmentCount = (await _simucraftContext.Skills
                .Where(r => r.RulesetId == rulesetId &&
                            r.UserId == userId)
                .ToListAsync()).Count;

            if (equipmentCount > 50)
                throw new MaxEntityException("Ruleset cannot contain more than 50 equipment.");

            var entity = _mapper.Map<Equipment>(request);
            entity.Id = Guid.NewGuid();
            entity.UserId = userId;
            entity.RulesetId = rulesetId;

            _simucraftContext.Equipment.Add(entity);
            await _simucraftContext.SaveChangesAsync();

            var response = _mapper.Map<EquipmentResponse>(entity);
            return response;
        }

        public async Task DeleteAsync(Guid userId, Guid entityId)
        {
            var existingEntity = await _simucraftContext.Equipment
                .SingleOrDefaultAsync(c =>
                    c.Id == entityId &&
                    c.UserId == userId);

            if (existingEntity == null)
                throw new NullReferenceException("Equipment not found.");

            _simucraftContext.Remove(existingEntity);
            await _simucraftContext.SaveChangesAsync();
        }

        public async Task<EquipmentResponse> GetByIdAsync(Guid userId, Guid entityId)
        {
            var entity = await _simucraftContext.Equipment
                .SingleOrDefaultAsync(c =>
                    c.Id == entityId &&
                    c.UserId == userId);

            if (entity == null)
                return null;

            var response = _mapper.Map<EquipmentResponse>(entity);
            return response;
        }

        public async Task<IEnumerable<EquipmentResponse>> GetByRulesetIdAsync(Guid userId, Guid rulesetId)
        {
            var entities = await _simucraftContext.Equipment
                .Where(c => c.UserId == userId &&
                            c.RulesetId == rulesetId)
                .ToListAsync();

            var responses = _mapper.Map<IEnumerable<EquipmentResponse>>(entities);
            return responses;
        }

        public async Task<EquipmentResponse> UpdateAsync(Guid userId, Guid rulesetId, Guid weaponId, EquipmentRequest request)
        {
            // TODO: I am storing small images in the document, determine if this needs to be moved to IBlobStorage.
            var entity = _mapper.Map<Equipment>(request);
            entity.Id = weaponId;
            entity.RulesetId = rulesetId;
            entity.UserId = userId;

            var existingEntity = await _simucraftContext.Equipment
                .SingleOrDefaultAsync(c =>
                    c.Id == weaponId &&
                    c.UserId == userId);

            if (existingEntity == null)
                throw new NullReferenceException("Equipment not found.");

            _simucraftContext.Equipment.Remove(existingEntity);
            _simucraftContext.Equipment.Add(entity);

            await _simucraftContext.SaveChangesAsync();

            var response = _mapper.Map<EquipmentResponse>(entity);
            return response;
        }
    }
}
