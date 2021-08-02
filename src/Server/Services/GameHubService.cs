using AutoMapper;
using Azure.Core;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.DataProtection.XmlEncryption;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.EntityFrameworkCore;
using Simucraft.Server.Common;
using Simucraft.Server.Core;
using Simucraft.Server.DataAccess;
using Simucraft.Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Simucraft.Server.Services
{
    public class GameHubService : IGameHubService
    {
        private readonly SimucraftContext _simucraftContext;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;

        public GameHubService(
            SimucraftContext simucraftContext,
            IMapper mapper,
            IEmailService emailService)
        {
            _simucraftContext = simucraftContext;
            _mapper = mapper;
            _emailService = emailService;
        }


        /// <summary>
        /// Adds a new chat message to the specified games logs.
        /// A limit is enforced, and will pop older messages when a new one is added.
        /// </summary>
        public async Task<IEnumerable<ChatMessageResponse>> EnqueueChatMessageAsync(Guid gameId, Guid userId, IEnumerable<ChatMessageRequest> chatMessageRequest)
        {
            var game = await _simucraftContext.Games.SingleAsync(g => g.Id == gameId);
            this.AuthorizeOrThrow(userId, game);

            var newChatMessages = new List<ChatMessage>();

            foreach (var message in chatMessageRequest)
            {
                var chatMessage = _mapper.Map<ChatMessage>(message);
                chatMessage.UserId = userId;
                chatMessage.DisplayName = game.AuthorizedUsers.First(u => u.UserId == userId).DisplayName;
                chatMessage.CreatedDateTime = DateTime.UtcNow;

                // TODO: Add system generated chat messages here.
                if (game.ChatMessages.Count > 1000)
                {
                    var lastMessage = game.ChatMessages
                        .OrderBy(c => c.CreatedDateTime)
                        .First();
                    game.ChatMessages.Remove(lastMessage);
                }

                game.ChatMessages.Add(chatMessage);

                newChatMessages.Add(chatMessage);
            }

            await _simucraftContext.SaveChangesAsync();

            var response = _mapper.Map<IEnumerable<ChatMessageResponse>>(newChatMessages);
            return response;
        }

        public async Task<IEnumerable<ChatMessageResponse>> GetChatMessagesAsync(Guid userId, Guid gameId)
        {
            var game = await _simucraftContext.Games.SingleAsync(g => g.Id == gameId);
            this.AuthorizeOrThrow(userId, game);

            var responses = _mapper.Map<IEnumerable<ChatMessageResponse>>(game.ChatMessages.OrderBy(c => c.CreatedDateTime));
            return responses;
        }

        public async Task<RulesetInformationResponse> GetRulesetInformationAsync(Guid userId, Guid gameId)
        {
            var game = await _simucraftContext.Games.SingleAsync(g => g.Id == gameId);
            this.AuthorizeOrThrow(userId, game);

            var ruleset = await _simucraftContext.Rulesets.SingleAsync(r => r.Id == game.RulesetId);
            var characters = await _simucraftContext.Characters
                .Where(c => c.RulesetId == game.RulesetId)
                .ToListAsync();

            var weapons = await _simucraftContext.Weapons
                .Where(w => w.RulesetId == game.RulesetId)
                .ToListAsync();

            var spells = await _simucraftContext.Spells
                .Where(s => s.RulesetId == game.RulesetId)
                .ToListAsync();

            var skills = await _simucraftContext.Skills
                .Where(s => s.RulesetId == game.RulesetId)
                .ToListAsync();

            var equipment = await _simucraftContext.Equipment
                .Where(e => e.RulesetId == game.RulesetId)
                .ToListAsync();

            var maps = await _simucraftContext.Maps
                .Where(m => m.RulesetId == game.RulesetId)
                .ToListAsync();

            return new RulesetInformationResponse
            {
                Id = game.RulesetId,
                MovementOffset = ruleset.MovementOffset,
                TurnOrderExpression = ruleset.TurnOrderExpression,
                IsOwner = userId == game.UserId,
                UserId = game.UserId,
                Characters = _mapper.Map<ICollection<CharacterResponse>>(characters),
                Maps = _mapper.Map<ICollection<MapResponse>>(maps),
                Equipment = _mapper.Map<ICollection<EquipmentResponse>>(equipment),
                Spells = _mapper.Map<ICollection<SpellResponse>>(spells),
                Skills = _mapper.Map<ICollection<SkillResponse>>(skills),
                Weapons = _mapper.Map<ICollection<WeaponResponse>>(weapons),
            };
        }

        public async Task<GameInformationResponse> GetGameInformationAsync(Guid userId, Guid gameId)
        {
            var game = await _simucraftContext.Games.SingleAsync(g => g.Id == gameId);
            this.AuthorizeOrThrow(userId, game);

            var response = _mapper.Map<GameInformationResponse>(game);
            return response;
        }

        public async Task<GameStateInformationResponse> GetGameStateInformationAsync(Guid userId, Guid gameId)
        {
            var game = await _simucraftContext.Games.SingleAsync(g => g.Id == gameId);
            this.AuthorizeOrThrow(userId, game);

            var response = _mapper.Map<GameStateInformationResponse>(game);
            return response;
        }

        public async Task InviteByEmailAsync(Guid userId, Guid gameId, string baseAddress, InviteRequest inviteRequest)
        {
            var game = await _simucraftContext.Games.SingleAsync(g => g.Id == gameId);
            if (game.UserId != userId)
                throw new InvalidOperationException("Unauthorized.");

            if (game.AuthorizedUsers.Any(u => u.EmailNormalized == inviteRequest.Email.ToUpper()) || game.InvitedUsers.Any(u => u.EmailNormalized == inviteRequest.Email.ToUpper()))
                throw new InvalidOperationException("Already in game.");

            var authorizedCount = game.AuthorizedUsers.Count();
            var invitedCount = game.InvitedUsers.Count();
            if (authorizedCount + invitedCount >= 10)
                throw new SubscriptionException("Cannot have more than 10 players in game.");

            // TODO: Validate the number of email being sent within a time frame, might need to prevent spam?

            var invitedUser = new InvitedUser
            {
                EmailNormalized = inviteRequest.Email.ToUpper(),
                ExpirationDateTime = DateTime.UtcNow.AddDays(1),
                Key = Guid.NewGuid().ToString("n"),
                CreatedDateTime = DateTime.UtcNow,
                Id = Guid.NewGuid(),
            };

            game.InvitedUsers.Add(invitedUser);


            var owner = game.AuthorizedUsers.First(u => u.UserId == game.UserId);

            // Email
            var url = $"{baseAddress}/rulesets/{game.RulesetId}/games/{game.Id}/invite/{invitedUser.Key}";
            var message = $"{owner.DisplayName} has invited you to play a Game in Simucraft! Click <a href=\"{url}\">here</a> to join and start playing.";

            await _emailService.EmailAsync("Simucraft Game Invite", message, invitedUser.EmailNormalized);

            await _simucraftContext.SaveChangesAsync();
        }

        public async Task RemoveInviteAsync(Guid userId, Guid gameId, string email)
        {
            var game = await _simucraftContext.Games.SingleAsync(g => g.Id == gameId);
            if (game.UserId != userId)
                throw new InvalidOperationException("Unauthorized.");

            var user = game.InvitedUsers.Single(u => u.EmailNormalized == email.ToUpper());
            game.InvitedUsers.Remove(user);

            await _simucraftContext.SaveChangesAsync();
        }

        public async Task RemoveAuthorizedUser(Guid userId, Guid gameId, Guid authorizedUserId)
        {
            var game = await _simucraftContext.Games.SingleAsync(g => g.Id == gameId);
            if (game.UserId != userId)
                throw new InvalidOperationException("Unauthorized.");

            if (game.UserId == authorizedUserId)
                throw new InvalidOperationException("Cannot be removed.");

            var user = game.AuthorizedUsers.Single(u => u.UserId == authorizedUserId);
            var invitedGame = _simucraftContext.InvitedGames.FirstAsync(g => g.GameId == gameId && g.UserId == authorizedUserId);

            game.AuthorizedUsers.Remove(user);
            _simucraftContext.Remove(invitedGame);

            await _simucraftContext.SaveChangesAsync();
        }

        public async Task<GameUserInformation> GetGameUserInformationAsync(Guid userId, Guid gameId)
        {
            var game = await _simucraftContext.Games.SingleAsync(g => g.Id == gameId);
            this.AuthorizeOrThrow(userId, game);

            return new GameUserInformation
            {
                MaxPlayerCount = 10,
                AuthorizedUsers = _mapper.Map<ICollection<AuthorizedUserResponse>>(game.AuthorizedUsers),
                InvitedUsers = _mapper.Map<ICollection<InvitedUserResponse>>(game.InvitedUsers),
            };
        }

        private void AuthorizeOrThrow(Guid userId, Game game)
        {
            if (!game.AuthorizedUsers.Any(u => u.UserId == userId))
                throw new InvalidOperationException("Unauthorized.");
        }
    }
}
