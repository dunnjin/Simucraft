using AutoMapper;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.EntityFrameworkCore;
using Simucraft.Server.Common;
using Simucraft.Server.Core;
using Simucraft.Server.DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simucraft.Server.Strategies
{
    public class RollGameCharacterStrategy : GameStateStrategy<RequestRollGameCharacter>
    {
        private readonly SimucraftContext _simucraftContext;
        private readonly IMapper _mapper;

        public RollGameCharacterStrategy(
            SimucraftContext simucraftContext,
            IMapper mapper)
        {
            _simucraftContext = simucraftContext;
            _mapper = mapper;
        }

        public override async Task<GameStateInformationResponse> RequestAsync(Guid gameId, Guid userId, RequestRollGameCharacter request)
        {
            var game = await _simucraftContext.Games.SingleAsync(g => g.Id == gameId);
            var ruleset = await _simucraftContext.Rulesets.SingleAsync(r => r.Id == game.RulesetId);
            var isOwner = game.UserId == userId;

            var gameCharacter = game.GameCharacters.Single(c => c.Id == request.GameCharacterId);
            if (gameCharacter.UserId != userId && !isOwner)
                throw new InvalidOperationException("Unauthorized.");


            var computationEngine = new DataTable();
            var combatLogs = new List<SystemMessage>();

            try
            {
                if (request.RulesetEntityType == RulesetEntityType.Skill)
                {
                    var skill = gameCharacter.Skills.Single(s => s.SkillId == request.RulesetEntityId);
                    if (string.IsNullOrEmpty(skill.Expression))
                        throw new InvalidOperationException("No expression.");
                    var result = gameCharacter.CalculateExpression(skill.Expression);

                    combatLogs.Add(new SystemMessage { Message = $"{gameCharacter.Name} used {skill.Name} with a {result.Result}.", Tooltip = result.VerboseResult });
                }
                else if(request.RulesetEntityType == RulesetEntityType.Weapon)
                {
                    var weapon = gameCharacter.Weapons.Single(w => w.WeaponId == request.RulesetEntityId);

                    var toHitResult = gameCharacter.CalculateExpression(weapon.HitChanceSelf);
                    var damageResult = gameCharacter.CalculateExpression(weapon.Damage, 0, toHitResult.Result);

                    var tooltip = $"Hit Chance: {toHitResult.VerboseResult}. Damage: {damageResult.VerboseResult}.";
                    var message = $"{gameCharacter.Name} used {weapon.Name} with a {toHitResult.Result} for {damageResult.Result} damage.";

                    combatLogs.Add(new SystemMessage { Message = message, Tooltip = tooltip });
                }
            }
            catch(Exception exception)
            {
                // Throw an error indicating being unable to perform action. Show equation witih error?
            }

            var response = _mapper.Map<GameStateInformationResponse>(game);
            response.SystemMessages = combatLogs;

            return response;
        }
    }
}
