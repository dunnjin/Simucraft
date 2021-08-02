using AutoMapper;
using Microsoft.AspNetCore.Authentication.AzureADB2C.UI.Pages.Internal;
using Microsoft.EntityFrameworkCore;
using Simucraft.Client.Core;
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
    public class RulesetService : IRulesetService
    {
        private readonly SimucraftContext _simucraftContext;
        private readonly IMapper _mapper;

        public RulesetService(
            SimucraftContext simucraftContext,
            IMapper mapper)
        {
            _simucraftContext = simucraftContext;
            _mapper = mapper;
        }

        public async Task<IEnumerable<RulesetResponse>> GetByUserIdAsync(Guid userId)
        {
            var rulesets = await _simucraftContext.Rulesets
                .Where(r => r.UserId == userId)
                .ToListAsync();

            var responses = _mapper.Map<IEnumerable<RulesetResponse>>(rulesets);
            return responses;
        }

        public async Task<RulesetResponse> GetByIdAsync(Guid userId, Guid rulesetId)
        {
            var ruleset = await _simucraftContext.Rulesets
                .SingleOrDefaultAsync(r =>
                    r.Id == rulesetId &&
                    r.UserId == userId);

            if (ruleset == null)
                return null;

            var response = _mapper.Map<RulesetResponse>(ruleset);
            return response;
        }

        public async Task<RulesetResponse> AddAsync(Guid userId, RulesetRequest rulesetRequest)
        {
            var ruleset = _mapper.Map<Ruleset>(rulesetRequest);
            ruleset.Id = Guid.NewGuid();
            ruleset.UserId = userId;

            var currentRulesets = await _simucraftContext.Rulesets
                .Where(r => r.UserId == userId)
                .ToListAsync();

            if (currentRulesets.Count >= 1)
                throw new SubscriptionException("Only one Ruleset is allowed at this time.");

            _simucraftContext.Rulesets.Add(ruleset);
            await _simucraftContext.SaveChangesAsync();

            var response = _mapper.Map<RulesetResponse>(ruleset);
            return response;
        }

        public async Task<RulesetResponse> ImportAsync(Guid userId, Guid templateId)
        {

            var template = new RulesetTemplates()
                .GetTemplate(templateId.ToString());

            if (template == null)
                throw new InvalidOperationException("Template not found.");

            var currentRulesets = await _simucraftContext.Rulesets
                .Where(r => r.UserId == userId)
                .ToListAsync();
            if (currentRulesets.Count >= 1)
                throw new SubscriptionException("Only one Ruleset is allowed at this time.");

            var ruleset = template.Ruleset(userId);
            var weapons = template.Weapons(userId, ruleset.Id);
            var characters = template.Characters(userId, ruleset.Id);

            _simucraftContext.Rulesets.Add(ruleset);
            _simucraftContext.Weapons.AddRange(weapons);
            _simucraftContext.Characters.AddRange(characters);

            await _simucraftContext.SaveChangesAsync();

            var response = _mapper.Map<RulesetResponse>(ruleset);
            return response;
        }

        public async Task<RulesetResponse> UpdateAsync(Guid userId, Guid rulesetId, RulesetRequest rulesetRequest)
        {
            var ruleset = _mapper.Map<Ruleset>(rulesetRequest);
            ruleset.Id = rulesetId;
            ruleset.UserId = userId;

            var existingRuleset = await _simucraftContext.Rulesets
                .SingleOrDefaultAsync(r =>
                    r.Id == ruleset.Id &&
                    r.UserId == userId);

            if (existingRuleset == null)
                throw new InvalidOperationException("Ruleset not found.");

            _simucraftContext.Rulesets.Remove(existingRuleset);
            _simucraftContext.Rulesets.Add(ruleset);

            await _simucraftContext.SaveChangesAsync();

            var response = _mapper.Map<RulesetResponse>(ruleset);
            return response;
        }

        public async Task DeleteAsync(Guid userId, Guid rulesetId)
        {
            var existingRuleset = await _simucraftContext.Rulesets
                .SingleOrDefaultAsync(r =>
                    r.Id == rulesetId &&
                    r.UserId == userId);

            if (existingRuleset == null)
                throw new InvalidOperationException("Ruleset not found.");

            var games = await _simucraftContext.Games
                .Where(g => g.RulesetId == rulesetId)
                .ToListAsync();

            var maps = await _simucraftContext.Maps
                .Where(m => m.RulesetId == rulesetId)
                .ToListAsync();

            var weapons = await _simucraftContext.Weapons
                .Where(w => w.RulesetId == rulesetId)
                .ToListAsync();

            var characters = await _simucraftContext.Characters
                .Where(c => c.RulesetId == rulesetId)
                .ToListAsync();

            _simucraftContext.Rulesets.Remove(existingRuleset);
            _simucraftContext.Games.RemoveRange(games);
            _simucraftContext.Weapons.RemoveRange(weapons);
            _simucraftContext.Characters.RemoveRange(characters);

            await _simucraftContext.SaveChangesAsync();
        }
    }
}
