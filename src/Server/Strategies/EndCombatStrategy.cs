using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Simucraft.Server.Core;
using Simucraft.Server.DataAccess;
using Simucraft.Server.Models;
using System;
using System.Threading.Tasks;

namespace Simucraft.Server.Strategies
{
    public class EndCombatStrategy : GameStateStrategy<RequestEndCombat>
    {
        private readonly SimucraftContext _simucraftContext;
        private readonly IMapper _mapper;

        public EndCombatStrategy(
            SimucraftContext simucraftContext,
            IMapper mapper)
        {
            _simucraftContext = simucraftContext;
            _mapper = mapper;
        }

        public override async Task<GameStateInformationResponse> RequestAsync(Guid gameId, Guid userId, RequestEndCombat request)
        {
            var game = await _simucraftContext.Games.SingleAsync(g => g.Id == gameId);
            if (game.UserId != userId)
                throw new InvalidOperationException("Unauthorized.");

            game.GameStateMode = GameStateMode.None;
            game.CurrentTurnId = null;

            await _simucraftContext.SaveChangesAsync();

            var response = _mapper.Map<GameStateInformationResponse>(game);
            return response;
        }
    }
}
