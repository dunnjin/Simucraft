using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Simucraft.Client.Common;
using Simucraft.Client.Models;
using Simucraft.Client.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Simucraft.Client.Components
{
    public partial class GameCharacterBook
    {
        private string _emailError;
        private bool _isSaving;
        private bool _isInviteView;

        private Guid? _selectedAuthorizedUserId;
        private string _selectedPendingInvite;
        private GameCharacterBookView _gameCharacterBookView;
        private EditContextValidator _editContextValidator;
        private InvitedUser _inviteRequest;

        [Parameter]
        public RulesetInformation Ruleset { get; set; }

        [Parameter]
        public GameUserInformation UserInformation { get; set; }

        [Parameter]
        public GameInformation Game { get; set; }

        [Parameter]
        public GameStateInformation GameState { get; set; }

        [Parameter]
        public ClientStateInformation ClientStateInformation { get; set; }

        [Parameter]
        public GameHubService GameHubService { get; set; }

        [Inject]
        private IJSRuntime JSRuntime { get; set; }

        protected override void OnInitialized()
        {
            _inviteRequest = InvitedUser.Empty;
            _gameCharacterBookView = GameCharacterBookView.TurnOrder;
            _editContextValidator = new EditContextValidator(_inviteRequest);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await this.JSRuntime.InvokeVoidAsync(Scripts.Semantic.DROPDOWN);

            if (!firstRender)
                return;

            await this.JSRuntime.InvokeVoidAsync(Scripts.Game.RESIZE_UI);
        }

        private async Task PlaceGameCharacterAsync()
        {
            try
            {
                if (!this.Game.MapId.HasValue)
                    return;

                var gameCharacter = this.GameState.GameCharacters.FirstOrDefault(c => c.Id == this.ClientStateInformation.SelectedGameCharacterId);
                if (gameCharacter == null)
                    return;

                this.ClientStateInformation.PlacementGameCharacter = gameCharacter;
                await this.JSRuntime.InvokeVoidAsync(Scripts.Game.RENDER_CLIENTSTATE, this.ClientStateInformation);
            }
            catch(Exception exception)
            {
                Console.WriteLine(exception.ToString());
            }
        }

        private async Task FavoriteAsync(Guid characterId)
        {
            try
            {
                var gameCharacter = this.GameState.GameCharacters.FirstOrDefault(c => c.Id == characterId);
                if (gameCharacter == null)
                    return;

                gameCharacter.IsFavorite = !gameCharacter.IsFavorite;

                await this.GameHubService.SendAsync("UpdateGameCharacter", new { gameCharacter });
            }
            catch(Exception exception)
            {
                Console.Write(exception.ToString());
            }
        }

        private async Task SendUserAsync(Guid userId, Guid characterId)
        {
            try
            {
                var gameCharacter = this.GameState.GameCharacters.FirstOrDefault(c => c.Id == characterId);
                if (gameCharacter == null)
                    return;

                var user = this.UserInformation.AuthorizedUsers.FirstOrDefault(u => u.UserId == userId);
                if (user == null)
                    return;

                if (gameCharacter.UserId == user.UserId)
                    return;

                gameCharacter.UserId = user.UserId;

                await this.GameHubService.SendAsync("UpdateGameCharacter", new { gameCharacter });
            }
            catch(Exception exception)
            {
                Console.Write(exception.ToString());
            }
        }

        private async Task InviteEmailAsync()
        {
            try
            {
                _isSaving = true;

                var isValidated = _editContextValidator.Validate();
                if (!isValidated)
                    return;

                if ((this.UserInformation.AuthorizedUsers.Count + this.UserInformation.InvitedUsers.Count) >= this.UserInformation.MaxPlayerCount)
                    throw new InvalidOperationException("Cannot have more than 10 players in game.");

                var email = _inviteRequest.Email.ToUpper();

                if (this.UserInformation.AuthorizedUsers.Any(u => u.EmailNormalized == email) || this.UserInformation.InvitedUsers.Any(u => u.EmailNormalized == email))
                    throw new InvalidOperationException("Email already added.");

                await this.GameHubService.SendAsync("InviteEmail", _inviteRequest);

                _inviteRequest.Email = null;
            }
            catch(InvalidOperationException invalidOperationException)
            {
                _emailError = invalidOperationException.Message;
            }
            catch(Exception exception)
            {
                Console.WriteLine(exception.ToString());
            }
            finally
            {
                _isSaving = false;
            }
        }

        private void ToggleNavigation(GameCharacterBookView gameCharacterBookView)
        {
            _gameCharacterBookView = gameCharacterBookView;
            _inviteRequest = InvitedUser.Empty;
            _editContextValidator = new EditContextValidator(_inviteRequest);
        }

        private async Task RemovePlayerAsync()
        {
            try
            {
                if (this.Ruleset.UserId == _selectedAuthorizedUserId)
                    return;

                var authorizedUser = this.UserInformation.AuthorizedUsers.First(u => u.UserId == _selectedAuthorizedUserId);
                await this.GameHubService.SendAsync("RemovePlayer", authorizedUser);
            }
            catch(Exception exception)
            {
                Console.WriteLine(exception.ToString());
            }
        }

        private async Task RemoveInviteAsync()
        {
            try
            {
                var invitedUser = this.UserInformation.InvitedUsers.First(u => u.EmailNormalized == _selectedPendingInvite);
                await this.GameHubService.SendAsync("RemoveInvite", invitedUser);
            }
            catch(Exception exception)
            {
                Console.WriteLine(exception.ToString());
            }
        }

        private async Task SelectGameCharacterId(Guid characterId)
        {
            try
            {
                var gameCharacter = this.GameState.GameCharacters.First(c => c.Id == characterId);
                if (gameCharacter.UserId != this.ClientStateInformation.UserId && !this.Ruleset.IsOwner)
                    return;

                this.ClientStateInformation.SelectedGameCharacterId = this.ClientStateInformation.SelectedGameCharacterId == characterId ? null : (Guid?)characterId;

                await this.JSRuntime.InvokeVoidAsync(Scripts.Game.RENDER_CLIENTSTATE, this.ClientStateInformation);
            }
            catch(Exception exception)
            {
                Console.WriteLine(exception.ToString());
            }
        }
    }
}
