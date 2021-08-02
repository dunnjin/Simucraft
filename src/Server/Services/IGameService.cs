using Simucraft.Server.Models;
using Simucraft.Server.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Simucraft.Server.Services
{
    public interface IGameService
    {
        Task<IEnumerable<GameResponse>> GetByUserIdAsync(Guid userId);
        Task<IEnumerable<GameResponse>> GetByInvitedAsync(Guid userId);

        Task<GameResponse> GetByIdAsync(Guid userId, Guid gameId);

        Task<GameResponse> AddAsync(Guid rulesetId, AuthorizedUserRequest authorizedUserRequest, GameRequest gameRequest);

        Task<GameResponse> UpdateAsync(Guid userId, Guid rulesetId, Guid gameId, GameRequest gameRequest);

        Task DeleteAsync(Guid userId, Guid gameId);

        Task AcceptInviteAsync(AuthorizedUserRequest authorizedUserRequest, Guid rulesetId, Guid gameId, string inviteId);

        //Task<IEnumerable<ChatMessageResponse>> GetChatMessagesAsync(Guid userId, Guid gameId);
        //Task<ChatMessageResponse> AddChatMessagAsync(string displayName, ChatMessageRequest chatMessageRequest);
    }
}
