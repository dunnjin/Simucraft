using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Simucraft.Server.Core;
using Simucraft.Server.DataAccess;
using Simucraft.Server.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Simucraft.Server.Strategies
{
    public class StartCombatStrategy : GameStateStrategy<RequestStartCombat>
    {
        private readonly SimucraftContext _simucraftContext;
        private readonly IMapper _mapper;

        public StartCombatStrategy(
            SimucraftContext simucraftContext,
            IMapper mapper)
        {
            _simucraftContext = simucraftContext;
            _mapper = mapper;
        }

        public override async Task<GameStateInformationResponse> RequestAsync(Guid gameId, Guid userId, RequestStartCombat request)
        {
            var game = await _simucraftContext.Games.SingleAsync(g => g.Id == gameId);
            var ruleset = await _simucraftContext.Rulesets.SingleAsync(r => r.Id == game.RulesetId);

            if (game.UserId != userId)
                throw new InvalidOperationException("Unauthorized.");

            // If there aren't any characters on the map prevent combat.
            if (game.GameCharacters.Any(c => c.IsVisible))
            {
                game.GameStateMode = GameStateMode.Combat;

                // Create new turn orders for every character.
                foreach (var gameCharacter in game.GameCharacters)
                    gameCharacter.TurnOrder = ruleset.GetTurnOrder(gameCharacter);

                game.GameCharacters = game.GameCharacters
                    .OrderByDescending(c => c.TurnOrder)
                    .ThenBy(c => c.Id)
                    .ToList();

                game.CurrentTurnId = game.GameCharacters.FirstOrDefault(c => c.IsVisible)?.Id;

                await _simucraftContext.SaveChangesAsync();
            }

            var response = _mapper.Map<GameStateInformationResponse>(game);
            return response;
        }
    }
}
