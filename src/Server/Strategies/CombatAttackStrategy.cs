using AutoMapper;
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
    public class CombatAttackStrategy : GameStateStrategy<RequestCombatAttack>
    {
        private readonly SimucraftContext _simucraftContext;
        private readonly IMapper _mapper;

        public CombatAttackStrategy(
            SimucraftContext simucraftContext,
            IMapper mapper)
        {
            _simucraftContext = simucraftContext;
            _mapper = mapper;
        }

        public override async Task<GameStateInformationResponse> RequestAsync(Guid gameId, Guid userId, RequestCombatAttack request)
        {
            var game = await _simucraftContext.Games.SingleAsync(g => g.Id == gameId);
            var ruleset = await _simucraftContext.Rulesets.SingleAsync(r => r.Id == game.RulesetId);
            var isOwner = game.UserId == userId;

            /**
             * TODO: Determine type of attack
             *       Generate shape
             *       Get affecting game characters
             *       Apply effect.
             */

            var gameCharacter = game.GameCharacters.Single(c => c.Id == request.GameCharacterId);
            if (gameCharacter.UserId != userId && !isOwner)
                throw new InvalidOperationException("Unauthorized.");

            var targetGameCharacters = game.GameCharacters.Where(c => c.X == request.TargetX && c.Y == request.TargetY);

            var weapon = gameCharacter.Weapons.Single(w => w.WeaponId == request.RulesetEntityId);
            var computationEngine = new DataTable();

            var combatLogs = new List<SystemMessage>();

            var hitChanceResult = gameCharacter.CalculateExpression(weapon.HitChanceSelf);
            var criticalHitChanceResult = gameCharacter.CalculateExpression(weapon.CriticalChanceSelf, 0, hitChanceResult.Result); 

            var damageResult = gameCharacter.CalculateExpression(weapon.Damage, 0, hitChanceResult.Result);
            var criticalDamageResult = gameCharacter.CalculateExpression(weapon.CriticalDamage, hitChanceResult.Result, damageResult.Result);

            foreach(var target in targetGameCharacters)
            {
                var targetHitChanceResult = target.CalculateExpression(weapon.HitChanceTarget, hitChanceResult.Result, damageResult.Result);
                var targetCriticalHitChanceResult = target.CalculateExpression(weapon.CriticalChanceTarget, hitChanceResult.Result, damageResult.Result);

                var hits = computationEngine.Compute($"{hitChanceResult.Result} {weapon.HitChanceOperator} {targetHitChanceResult.Result}", null) as bool?;
                var criticalHits = computationEngine.Compute($"{criticalHitChanceResult.Result} {weapon.CriticalChanceOperator} {targetCriticalHitChanceResult.Result}", null) as bool?;

                var totalDamage = !criticalHits.Value ? damageResult.Result : criticalDamageResult.Result;

                var message = new StringBuilder()
                    .Append($"{gameCharacter.Name} used {weapon.Name} and ")
                    .AppendWhen("critically hit ", s => criticalHits.Value)
                    .AppendWhen("hit ", s => hits.Value && !criticalHits.Value)
                    .AppendWhen("missed ", s => !hits.Value)
                    .Append($"{target.Name} with a {hitChanceResult.Result}")
                    .AppendWhen(".", s => !hits.Value)
                    .AppendWhen($" for {totalDamage} damage", s => hits.Value)
                    .ToString();

                var tooltip = new StringBuilder()
                    .AppendWhen($"Critical Hit Chance: {hitChanceResult.VerboseResult} & {criticalHitChanceResult.VerboseResult}.", s => criticalHits.Value)
                    .AppendWhen($"Hit Chance: {hitChanceResult.VerboseResult}.", s => !criticalHits.Value)
                    .AppendWhen($" Damage: {damageResult.VerboseResult}.", s => hits.Value && !criticalHits.Value)
                    .AppendWhen($" Critical Damage: {damageResult.VerboseResult} + {criticalDamageResult.VerboseResult}.", s => criticalHits.Value)
                    .ToString();

                combatLogs.Add(new SystemMessage { Message = message, Tooltip = tooltip });

                if (hits.Value && ruleset.AutoApplyDamage)
                    target.HealthPoints = Math.Clamp(target.HealthPoints - totalDamage, 0, Int32.MaxValue);
            }

            await _simucraftContext.SaveChangesAsync();

            var response = _mapper.Map<GameStateInformationResponse>(game);
            response.SystemMessages = combatLogs;

            return response;
        }
    }
}
