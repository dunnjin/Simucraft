using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Simucraft.Server.Core;
using Simucraft.Server.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Simucraft.Server.Strategies
{
    public class EndTurnStrategy : GameStateStrategy<RequestEndTurn>
    {
        private readonly SimucraftContext _simucraftContext;
        private readonly IMapper _mapper;

        public EndTurnStrategy(
            SimucraftContext simucraftContext,
            IMapper mapper)
        {
            _simucraftContext = simucraftContext;
            _mapper = mapper;
        }

        public override async Task<GameStateInformationResponse> RequestAsync(Guid gameId, Guid userId, RequestEndTurn request)
        {
            var game = await _simucraftContext.Games.SingleAsync(g => g.Id == gameId);
            var isOwner = game.UserId == userId;

            var logs = new List<SystemMessage>();
            var gameCharacter = game.GameCharacters.SingleOrDefault(c => c.Id == game.CurrentTurnId);
            if (gameCharacter != null)
            {
                logs.Add(new SystemMessage { Message = $"{gameCharacter.Name} ended turn." });

                if (gameCharacter.UserId != userId && !isOwner)
                    throw new InvalidOperationException("Unauthorized.");

                game.CurrentTurnId = game.GetNextTurnId();

                //var nextGameCharacter = game.GameCharacters.SingleOrDefault(c => c.Id == game.CurrentTurnId);
                //if (nextGameCharacter != null)
                //    logs.Add(new SystemMessage { Message = $"{nextGameCharacter.Name} started turn." });
            }

            await _simucraftContext.SaveChangesAsync();

            var response = _mapper.Map<GameStateInformationResponse>(game);
            response.SystemMessages = logs;
            return response;
        }
    }
}
