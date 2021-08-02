using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Simucraft.Server.Common;
using Simucraft.Server.Core;
using Simucraft.Server.Services;
using Simucraft.Server.Strategies;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Resources;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Simucraft.Server.Hubs
{
    [Authorize]
    public class GameHub : Hub
    {
        private readonly IGameHubService _gameHubService;
        private readonly IGameStrategyService _gameStrategyService;
        private readonly IGameStateStrategyService _gameStateStrategyService;

        public GameHub(
            IGameHubService gameHubService,
            IGameStrategyService gameStrategyService,
            IGameStateStrategyService gameStateStrategyService)
        {
            _gameHubService = gameHubService;
            _gameStrategyService = gameStrategyService;
            _gameStateStrategyService = gameStateStrategyService;
        }
      

        public async Task SendChatMessage(ChatMessageRequest chatMessageRequest)
        {
            try
            {
                var gameId = base.Context.GetHttpContext().Request.GetGameId();
                var userId = base.Context.User.GetId();

                var response = await _gameHubService.EnqueueChatMessageAsync(gameId, userId, new ChatMessageRequest[] { chatMessageRequest });

                await base.Clients
                    .Group(gameId.ToString())
                    .SendAsync("OnChatMessages", response);
            }
            catch(InvalidOperationException invalidOperationException)
            {
                await this.SendErrorAsync(401, invalidOperationException.Message);
            }
            catch(Exception exception)
            {
                throw;
            }
        }

        public async Task InviteEmail(InviteRequest inviteRequest)
        {
            var gameId = base.Context.GetHttpContext().Request.GetGameId();
            var userId = base.Context.GetHttpContext().User.GetId();

            var scheme = base.Context.GetHttpContext().Request.Scheme;
            var baseAddress = $"{scheme}://{base.Context.GetHttpContext().Request.Host.Value}";

            await _gameHubService.InviteByEmailAsync(userId, gameId, baseAddress, inviteRequest);

            await base.Clients
                .Group(gameId.ToString())
                .SendAsync("OnGameUserInformation", await _gameHubService.GetGameUserInformationAsync(userId, gameId));
        }

        public async Task RemovePlayer(AuthorizedUserRequest authorizedUserRequest)
        {
            try
            {
                var gameId = base.Context.GetHttpContext().Request.GetGameId();
                var userId = base.Context.GetHttpContext().User.GetId();

                await _gameHubService.RemoveAuthorizedUser(userId, gameId, authorizedUserRequest.UserId);

                await base.Clients
                    .Group(gameId.ToString())
                    .SendAsync("OnGameUserInformation", await _gameHubService.GetGameUserInformationAsync(userId, gameId));
            }
            catch(Exception exception)
            {
                throw;
            }
        }

        public async Task RemoveInvite(InvitedUserRequest inviteRequest)
        {
            try
            {
                var gameId = base.Context.GetHttpContext().Request.GetGameId();
                var userId = base.Context.GetHttpContext().User.GetId();

                await _gameHubService.RemoveInviteAsync(userId, gameId, inviteRequest.EmailNormalized);

                await base.Clients
                    .Group(gameId.ToString())
                    .SendAsync("OnGameUserInformation", await _gameHubService.GetGameUserInformationAsync(userId, gameId));
            }
            catch(Exception exception)
            {
                throw;
            }
        }

        public async Task LoadMap(RequestLoadMap requestLoadMap)
        {
            var userId = base.Context.User.GetId();
            // TODO: Validate user id.

            var gameId = base.Context.GetHttpContext().Request.GetGameId();

            try
            {

                await this.SendLoadingMessageAsync(gameId, "Retrieving Map...");

                var response = await _gameStrategyService.RequestAsync(gameId, requestLoadMap);
                await this.SendGameResponseAsync(gameId, response);

                var gameState = await _gameHubService.GetGameStateInformationAsync(userId, gameId);
                await this.SendGameStateResponseAsync(gameId, gameState);
            }
            catch(Exception exception)
            {
            }
            finally
            {
                await this.SendLoadingMessageAsync(gameId, null);
            }

            // TODO: Need to get game characters as well from hub service.
        }

        public Task AddCharacter(RequestAddCharacter request) => this.RequestGameStateAsync(request);
        public Task Placement(RequestPlacement request) => this.RequestGameStateAsync(request);
        public Task EndTurn(RequestEndTurn request) => this.RequestGameStateAsync(request);
        public Task CombatPlacement(RequestCombatPlacement request) => this.RequestGameStateAsync(request);
        public Task UpdateGameCharacter(RequestUpdateGameCharacter request) => this.RequestGameStateAsync(request);
        public Task RollGameCharacter(RequestRollGameCharacter request) => this.RequestGameStateAsync(request);
        public Task CombatMove(RequestCombatMove request) => this.RequestGameStateAsync(request);

        public async Task StartCombat(RequestStartCombat request)
        {
            var gameId = base.Context.GetHttpContext().Request.GetGameId();

            try
            {
                await this.SendLoadingMessageAsync(gameId, "Starting Combat...");
                await this.RequestGameStateAsync(request);
            }
            catch (Exception exception)
            {

            }
            finally
            {
                await this.SendLoadingMessageAsync(gameId, null);
            }
        }

        public async Task EndCombat(RequestEndCombat request)
        {
            var gameId = base.Context.GetHttpContext().Request.GetGameId();

            try
            {
                await this.SendLoadingMessageAsync(gameId, "Ending Combat...");
                await this.RequestGameStateAsync(request);
            }
            catch (Exception exception)
            {

            }
            finally
            {
                await this.SendLoadingMessageAsync(gameId, null);
            }
        }

        public async Task CombatAttack(RequestCombatAttack request) => await this.RequestGameStateAsync(request);

        private async Task RequestGameStateAsync<T>(T request)
            where T : IGameStateStrategyRequest
        {
            try
            {
                var userId = base.Context.User.GetId();
                // TODO: Validate user id.

                var gameId = base.Context.GetHttpContext().Request.GetGameId();

                var response = await _gameStateStrategyService.RequestAsync(gameId, userId, request);

                await this.SendGameStateResponseAsync(gameId, response);

                if (response.SystemMessages.Any())
                    await this.SendSystemMessagesAsync(response.SystemMessages);
            }
            catch(Exception exception)
            {
                throw;
            }
        }

        private async Task SendGameResponseAsync(Guid gameId, GameInformationResponse gameInformationResponse)
        {
            await base.Clients
                .Group(gameId.ToString())
                .SendAsync("OnGameInformation", gameInformationResponse);
        }

        private async Task SendGameStateResponseAsync(Guid gameId, GameStateInformationResponse gameStateInformationResponse)
        {
            await base.Clients
                .Group(gameId.ToString())
                .SendAsync("OnGameStateInformation", gameStateInformationResponse);
        }

        private async Task SendLoadingMessageAsync(Guid gameId, string message)
        {
            await base.Clients
                .Group(gameId.ToString())
                .SendAsync("OnLoadingMessage", new { message } );
        }

        private async Task SendErrorAsync(int statusCode, string message)
        {
            await base.Clients.Caller.SendAsync("OnError",
                new
                {
                    StatusCode = statusCode,
                    Message = message,
                });
        }

        private async Task SendSystemMessagesAsync(IEnumerable<SystemMessage> messages)
        {
            var gameId = base.Context.GetHttpContext().Request.GetGameId();
            var userId = base.Context.GetHttpContext().User.GetId();

            var response = await _gameHubService.EnqueueChatMessageAsync(
                gameId, 
                userId,
                messages.Select(m => 
                    new ChatMessageRequest
                    {
                        Message = m.Message,
                        IsSystem = true,
                        Tooltip = m.Tooltip,
                    }));

            await base.Clients
                .Group(gameId.ToString())
                .SendAsync("OnChatMessages", response);
        }

     

        #region Hub
        public override async Task OnConnectedAsync()
        {

            try
            {
                var userId = base.Context.User.GetId();
                var displayName = base.Context.User.GetDisplayName();
                var gameId = base.Context.GetHttpContext().Request.GetGameId();

                // Create game group.
                await base.Groups.AddToGroupAsync(base.Context.ConnectionId, gameId.ToString());

                // Initialize the client. 
                await base.Clients.Caller.SendAsync("OnLoadingMessage", new { Message = "Initializing Game..." });

                await base.Clients.Caller.SendAsync("OnChatMessages", await _gameHubService.GetChatMessagesAsync(userId, gameId));
                await base.Clients.Caller.SendAsync("OnRulesetInformation", await _gameHubService.GetRulesetInformationAsync(userId, gameId));
                await base.Clients.Caller.SendAsync("OnGameInformation", await _gameHubService.GetGameInformationAsync(userId, gameId));
                await base.Clients.Caller.SendAsync("OnGameStateInformation", await _gameHubService.GetGameStateInformationAsync(userId, gameId));
                /**
                 * Send to entire game since there can be new users connecting for the first time,
                 * and connections in the current game need to be updated.
                 */
                await base.Clients
                    .Group(gameId.ToString())
                    .SendAsync("OnGameUserInformation", await _gameHubService.GetGameUserInformationAsync(userId, gameId));
                await this.SendSystemMessagesAsync(new SystemMessage[] { new SystemMessage { Message = $"Connected" } });

                await base.Clients.Caller.SendAsync("OnLoadingMessage", new { Message = default(string) });
            }
            catch (InvalidOperationException invalidOperationException)
            {
                await this.SendErrorAsync(401, invalidOperationException.Message);
            }
            catch(Exception exception)
            {
                // TODO: Send an error to allow client to show message to refresh or something.
            }
            finally
            {
            }
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            try
            {
                var gameId = base.Context.GetHttpContext().Request.GetGameId();
                var displayName = base.Context.User.GetDisplayName();

                await this.SendSystemMessagesAsync(new SystemMessage[] { new SystemMessage { Message = $"Disconnected" } });
            }
            catch (Exception innerException)
            {

            }
        }
        #endregion
    }
}
