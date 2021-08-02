using Simucraft.Server.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Simucraft.Server.Services
{
    public interface IGameHubService
    {
        Task<IEnumerable<ChatMessageResponse>> EnqueueChatMessageAsync(Guid gameId, Guid userId, IEnumerable<ChatMessageRequest> chatMessageRequest);
        Task<IEnumerable<ChatMessageResponse>> GetChatMessagesAsync(Guid userId, Guid gameId);

        Task<RulesetInformationResponse> GetRulesetInformationAsync(Guid userId, Guid gameId);

        Task<GameInformationResponse> GetGameInformationAsync(Guid userId, Guid gameId);

        Task<GameStateInformationResponse> GetGameStateInformationAsync(Guid userId, Guid gameId);

        Task InviteByEmailAsync(Guid userId, Guid gameId, string baseAddress, InviteRequest inviteRequest);

        Task RemoveInviteAsync(Guid userId, Guid gameId, string key);

        Task RemoveAuthorizedUser(Guid userId, Guid gameId, Guid authorizedUserId);

        Task<GameUserInformation> GetGameUserInformationAsync(Guid userId, Guid gameId);
    }
}
