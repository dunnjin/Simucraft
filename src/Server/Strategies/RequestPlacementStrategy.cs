using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Simucraft.Server.Core;
using Simucraft.Server.DataAccess;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Simucraft.Server.Strategies
{
    public class RequestPlacementStrategy : GameStateStrategy<RequestPlacement>
    {
        private readonly SimucraftContext _simucraftContext;
        private readonly IMapper _mapper;

        public RequestPlacementStrategy(
            SimucraftContext simucraftContext,
            IMapper mapper)
        {
            _simucraftContext = simucraftContext;
            _mapper = mapper;
        }

        public override async Task<GameStateInformationResponse> RequestAsync(Guid gameId, Guid userId, RequestPlacement request)
        {
            var game = await _simucraftContext.Games.SingleAsync(g => g.Id == gameId);
            var isOwner = game.UserId == userId;

            foreach (var gameCharacter in game.GameCharacters.Where(gc => request.GameCharacters.Any(c => c.Id == gc.Id)))
            {
                if (gameCharacter.UserId != userId && !isOwner)
                    throw new InvalidOperationException("Unauthorized.");

                var character = request.GameCharacters.Single(c => c.Id == gameCharacter.Id);
                _mapper.Map(character, gameCharacter);
            }

            await _simucraftContext.SaveChangesAsync();

            var response = _mapper.Map<GameStateInformationResponse>(game);
            return response;
        }
    }
}
