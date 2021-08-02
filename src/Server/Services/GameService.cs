using AutoMapper;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
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
    public class GameService : IGameService
    {
        private readonly SimucraftContext _simucraftContext;
        private readonly IMapper _mapper;

        public GameService(
            SimucraftContext simucraftContext,
            IMapper mapper)
        {
            _simucraftContext = simucraftContext;
            _mapper = mapper;
        }

        public async Task<IEnumerable<GameResponse>> GetByUserIdAsync(Guid userId)
        {
            var games = await _simucraftContext.Games
                .Where(g => g.UserId == userId)
                .ToListAsync();

            var gameResponses = _mapper.Map<IEnumerable<GameResponse>>(games);

            return gameResponses;
        }

        public async Task<IEnumerable<GameResponse>> GetByInvitedAsync(Guid userId)
        {
            var invitedGames = await _simucraftContext.InvitedGames
                .Where(g => g.UserId == userId)
                .ToListAsync();

            var invitedGameIds = invitedGames.Select(g => g.GameId);
            var games = await _simucraftContext.Games
                .Where(g => invitedGameIds.Contains(g.Id))
                .ToListAsync();

            var gameResponses = _mapper.Map<IEnumerable<GameResponse>>(games.Where(u => u.UserId != userId));
            return gameResponses;
        }

        public async Task<GameResponse> GetByIdAsync(Guid userId, Guid gameId)
        {
            var game = await _simucraftContext.Games
                .SingleOrDefaultAsync(g => g.Id == gameId &&
                                           g.UserId == userId);

            if (game == null)
                return null;

            var gameResponse = _mapper.Map<GameResponse>(game);

            return gameResponse;
        }

        public async Task<GameResponse> AddAsync(Guid rulesetId, AuthorizedUserRequest authorizedUserRequest, GameRequest gameRequest)
        {
            var ruleset = await _simucraftContext.Rulesets
                .SingleOrDefaultAsync(r => r.Id == rulesetId &&
                                           r.UserId == authorizedUserRequest.UserId);

            if (ruleset == null)
                throw new InvalidOperationException("Ruleset not found.");

            var authorizedUser = _mapper.Map<AuthorizedUser>(authorizedUserRequest);
            authorizedUser.Role = "admin";

            var game = _mapper.Map<Models.Game>(gameRequest);
            game.Id = Guid.NewGuid();
            game.UserId = authorizedUserRequest.UserId;
            game.RulesetId = rulesetId;
            //game.MovementOffset = ruleset.MovementOffset;
            //game.TurnOrderFormula = ruleset.TurnOrderFormula;
            game.AuthorizedUsers.Add(authorizedUser);

            _simucraftContext.Games.Add(game);
            await _simucraftContext.SaveChangesAsync();

            var gameResponse = _mapper.Map<GameResponse>(game);
            return gameResponse;
        }

        public async Task<GameResponse> UpdateAsync(Guid userId, Guid rulesetId, Guid mapId, GameRequest gameRequest)
        {
            var game = _mapper.Map<Models.Game>(gameRequest);
            game.Id = mapId;
            game.UserId = userId;
            game.RulesetId = rulesetId;

            var existingGame = await _simucraftContext.Games
                .SingleOrDefaultAsync(g => g.Id == game.Id &&
                                           g.UserId == userId);

            if (existingGame == null)
                throw new InvalidOperationException("Game not found.");

            _simucraftContext.Games.Remove(existingGame);
            _simucraftContext.Games.Add(game);

            await _simucraftContext.SaveChangesAsync();

            var gameResponse = _mapper.Map<GameResponse>(game);

            return gameResponse;
        }

        public async Task DeleteAsync(Guid userId, Guid gameId)
        {
            var existingGame = await _simucraftContext.Games
                .SingleOrDefaultAsync(g => g.Id == gameId &&
                                           g.UserId == userId);

            if (existingGame == null)
                throw new InvalidOperationException("Game not found.");

            _simucraftContext.Remove(existingGame);

            await _simucraftContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<ChatMessageResponse>> GetChatMessagesAsync(Guid userId, Guid gameId)
        {
            var game = await _simucraftContext.Games
                .SingleOrDefaultAsync(g => g.Id == gameId &&
                                           g.UserId == userId);

            if (game == null)
                throw new InvalidOperationException("Game not found.");

            var chatMessages = _mapper.Map<IEnumerable<ChatMessageResponse>>(game.ChatMessages.OrderBy(c => c.CreatedDateTime));
            return chatMessages;
        }

        public async Task AcceptInviteAsync(AuthorizedUserRequest authorizedUserRequest, Guid rulesetId, Guid gameId, string inviteId)
        {
            var userId = authorizedUserRequest.UserId;
            var game = await _simucraftContext.Games.SingleAsync(g => g.Id == gameId);

            if(game.RulesetId != rulesetId)
                throw new InvalidOperationException("Invalid Invite.");

            var user = game.InvitedUsers.FirstOrDefault(u => u.EmailNormalized == authorizedUserRequest.EmailNormalized && u.Key == inviteId);
            if (user == null)
                throw new InvalidOperationException("Invalid Invite.");

            game.InvitedUsers.Remove(user);

            var authorizedUser = new AuthorizedUser
            {
                DisplayName = authorizedUserRequest.DisplayName,
                UserId = authorizedUserRequest.UserId,
                EmailNormalized = authorizedUserRequest.EmailNormalized,
                Role = "player",
            };

            var invitedGame = new InvitedGame
            {
                Id = Guid.NewGuid(),
                GameId = game.Id,
                UserId = authorizedUserRequest.UserId,
            };

            var isValidated = DateTime.UtcNow <= user.ExpirationDateTime;

            if (isValidated)
            {
                game.AuthorizedUsers.Add(authorizedUser);
                _simucraftContext.InvitedGames.Add(invitedGame);
            }

            await _simucraftContext.SaveChangesAsync();

            if (!isValidated)
                throw new InvalidOperationException("Invite Expired.");
        }
    }
}
