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
    public class SkillService : ISkillService
    {
        private readonly SimucraftContext _simucraftContext;
        private readonly IMapper _mapper;

        public SkillService(
            SimucraftContext simucraftContext,
            IMapper mapper)
        {
            _simucraftContext = simucraftContext;
            _mapper = mapper;
        }

        public async Task<SkillResponse> AddAsync(Guid userId, Guid rulesetId, SkillRequest request)
        {
            var ruleset = await _simucraftContext.Rulesets
                .SingleOrDefaultAsync(r =>
                    r.Id == rulesetId &&
                    r.UserId == userId);

            if (ruleset == null)
                throw new NullReferenceException("Ruleset not found.");

            var skillCount = (await _simucraftContext.Skills
                .Where(r => r.RulesetId == rulesetId &&
                            r.UserId == userId)
                .ToListAsync())
                .Count;

            if (skillCount > 50)
                throw new MaxEntityException("Ruleset cannot contain more than 50 Skills.");

            var entity = _mapper.Map<Skill>(request);
            entity.Id = Guid.NewGuid();
            entity.UserId = userId;
            entity.RulesetId = rulesetId;

            _simucraftContext.Skills.Add(entity);
            await _simucraftContext.SaveChangesAsync();

            var response = _mapper.Map<SkillResponse>(entity);
            return response;
        }

        public async Task DeleteAsync(Guid userId, Guid entityId)
        {
            var existingEntity = await _simucraftContext.Skills
                .SingleOrDefaultAsync(c =>
                    c.Id == entityId &&
                    c.UserId == userId);

            if (existingEntity == null)
                throw new NullReferenceException("Skill not found.");

            _simucraftContext.Remove(existingEntity);
            await _simucraftContext.SaveChangesAsync();
        }

        public async Task<SkillResponse> GetByIdAsync(Guid userId, Guid entityId)
        {
            var entity = await _simucraftContext.Skills
                .SingleOrDefaultAsync(c =>
                    c.Id == entityId &&
                    c.UserId == userId);

            if (entity == null)
                return null;

            var response = _mapper.Map<SkillResponse>(entity);
            return response;
        }

        public async Task<IEnumerable<SkillResponse>> GetByRulesetIdAsync(Guid userId, Guid rulesetId)
        {
            var entities = await _simucraftContext.Skills
                .Where(c => c.UserId == userId &&
                            c.RulesetId == rulesetId)
                .ToListAsync();

            var responses = _mapper.Map<IEnumerable<SkillResponse>>(entities);
            return responses;
        }

        public async Task<SkillResponse> UpdateAsync(Guid userId, Guid rulesetId, Guid entityId, SkillRequest request)
        {
            var entity = _mapper.Map<Skill>(request);
            entity.Id = entityId;
            entity.RulesetId = rulesetId;
            entity.UserId = userId;

            var existingEntity = await _simucraftContext.Skills
                .SingleOrDefaultAsync(c =>
                    c.Id == entityId &&
                    c.UserId == userId);

            if (existingEntity == null)
                throw new NullReferenceException("Skill not found.");

            _simucraftContext.Skills.Remove(existingEntity);
            _simucraftContext.Skills.Add(entity);

            await _simucraftContext.SaveChangesAsync();

            var response = _mapper.Map<SkillResponse>(entity);
            return response;
        }
    }
}
