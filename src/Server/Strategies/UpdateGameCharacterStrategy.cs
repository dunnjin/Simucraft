using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Simucraft.Server.Core;
using Simucraft.Server.DataAccess;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Simucraft.Server.Strategies
{
    public class UpdateGameCharacterStrategy : GameStateStrategy<RequestUpdateGameCharacter>
    {
        private readonly SimucraftContext _simucraftContext;
        private readonly IMapper _mapper;

        public UpdateGameCharacterStrategy(
            SimucraftContext simucraftContext,
            IMapper mapper)
        {
            _simucraftContext = simucraftContext;
            _mapper = mapper;
        }

        public override async Task<GameStateInformationResponse> RequestAsync(Guid gameId, Guid userId, RequestUpdateGameCharacter request)
        {
            var game = await _simucraftContext.Games.SingleAsync(g => g.Id == gameId);
            var isOwner = game.UserId == userId;

            var gameCharacter = game.GameCharacters.Single(gc => request.GameCharacter.Id == gc.Id);
            if (gameCharacter.UserId != userId && !isOwner)
                throw new InvalidOperationException("Unauthorized.");

            // TODO: Validation on incoming character.
            _mapper.Map(request.GameCharacter, gameCharacter);

            await _simucraftContext.SaveChangesAsync();

            var response = _mapper.Map<GameStateInformationResponse>(game);
            return response;
        }
    }
}
