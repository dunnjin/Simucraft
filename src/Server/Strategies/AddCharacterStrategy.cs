using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Simucraft.Server.Models;
using Simucraft.Server.Core;
using Simucraft.Server.DataAccess;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace Simucraft.Server.Strategies
{
    public class AddCharacterStrategy : GameStateStrategy<RequestAddCharacter>
    {
        private readonly SimucraftContext _simucraftContext;
        private readonly IMapper _mapper;

        public AddCharacterStrategy(
            SimucraftContext simucraftContext,
            IMapper mapper)
        {
            _simucraftContext = simucraftContext;
            _mapper = mapper;
        }

        public override async Task<GameStateInformationResponse> RequestAsync(Guid gameId, Guid userId, RequestAddCharacter request)
        {
            var game = await _simucraftContext.Games.SingleAsync(g => g.Id == gameId);
            if (game.UserId != userId)
                throw new InvalidOperationException("Unauthorized.");

            var character = await _simucraftContext.Characters.SingleAsync(c => c.Id == request.CharacterId);
            var ruleset = await _simucraftContext.Rulesets.SingleAsync(r => r.Id == game.RulesetId);
            var weapons = await _simucraftContext.Weapons
                .Where(w => w.RulesetId == ruleset.Id)
                .ToListAsync();

            var gameCharacter = _mapper.Map<GameCharacter>(character);
            gameCharacter.Id = Guid.NewGuid();
            gameCharacter.TurnOrder = ruleset.GetTurnOrder(gameCharacter);
            gameCharacter.HealthPoints = gameCharacter.CalculateExpression(gameCharacter.MaxHealthPoints).Result;
            gameCharacter.Weapons = _mapper.Map<ICollection<GameCharacterWeapon>>(weapons.Where(w => character.WeaponIds.Contains(w.Id)));

            game.GameCharacters.Add(gameCharacter);

            await _simucraftContext.SaveChangesAsync();

            var response = _mapper.Map<GameStateInformationResponse>(game);
            return response;
        }
    }
}
