using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Simucraft.Client.Common;
using Simucraft.Client.Core;
using Simucraft.Client.Models;
using Simucraft.Client.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Simucraft.Client.Pages
{
    [Authorize]
    public partial class GameView : ComponentBase, IDisposable
    {
        private RulesetInformation _rulesetInformation;
        private GameInformation _gameInformation;
        private GameUserInformation _gameUserInformation;
        private GameStateInformation _gameStateInformation;
        private ClientStateInformation _clientStateInformation;

        private ChatMessage _chatMessage;

        private bool _isLoading;
        private bool _isGridVisible;
        private bool _isConnected;
        private bool _isOwner;
        private bool _isLeftCollapsed;
        private bool _isRightCollapsed;
        private bool _showContextMenu;
        private bool _showHiddenGameCharacters;

        private ICollection<ChatMessage> _chatMessages = new List<ChatMessage>();

        private Guid? _visuallySelectedCharacterId;
        private Guid? _visuallySelectedMapId;
        private Guid? _visuallySelectedGameCharacterId;

        private string _errorMessage;
        private string _selectedZoom;
        private string _loadingMessage;
        private string _searchInput;

        private int _newGameCharactersCount = 0;

        private IList<string> _zoomIncrements = new List<string>();
        private GameViewOption _gameViewOption;
        private ClientState _clientState;

        [Inject]
        private IJSRuntime JSRuntime { get; set; }

        [Inject]
        private IGameService GameService { get; set; }

        [Inject]
        private NavigationManager NavigationManager { get; set; }

        [Inject]
        private GameHubService GameHubService { get; set; }

        [Inject]
        private AuthenticationStateProvider AuthenticationStateProvider { get; set; }

        public GameCharacter SelectedGameCharacter => _gameStateInformation.GameCharacters.FirstOrDefault(c => c.Id == _clientStateInformation.SelectedGameCharacterId);

        [Parameter]
        public string RulesetId { get; set; }

        [Parameter]
        public string Id { get; set; }

        protected override void OnInitialized()
        {
            try
            {
                // TODO: Need to set height for chatbar and lists on left side to prevent lists from scrolling past the viewport.
                _chatMessage = new ChatMessage();
                _rulesetInformation = RulesetInformation.Empty;
                _gameInformation = GameInformation.Empty;
                _gameStateInformation = GameStateInformation.Empty;
                _clientStateInformation = ClientStateInformation.Empty;
                _selectedZoom = "100";
                _isGridVisible = true;
                _zoomIncrements = new List<string> { "25", "50", "75", "100", "125", "150", "175", "200", "250", "300", "400" };
            }
            catch(Exception exception)
            {
                Console.WriteLine(exception.ToString());
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
                return;

            try
            {
                _isLoading = true;

                if (!Guid.TryParse(this.Id, out var gameId))
                    throw new InvalidOperationException("Invalid game id.");

                if (!Guid.TryParse(this.RulesetId, out var rulesetId))
                    throw new InvalidOperationException("Invalid ruleset id.");

                await this.JSRuntime.InvokeVoidAsync(Scripts.Game.INITIALIZE);
                await this.JSRuntime.InvokeVoidAsync(Scripts.Semantic.DROPDOWN);

                _clientStateInformation.UserId = (await this.AuthenticationStateProvider.GetAuthenticationStateAsync()).User.GetId();

                // game.js needs authorized users, to remove transfromer/events for illegal users.

                this.GameHubService.Disconnected += () => 
                { 
                    _isConnected = false;
                    _chatMessages.Clear();

                    base.StateHasChanged();
                    return Task.CompletedTask;
                };

                this.GameHubService.Connected += () => 
                {
                    _isConnected = true;
                    base.StateHasChanged();
                    return Task.CompletedTask;
                };

                await this.GameHubService
                    .On<ErrorMessage>(nameof(this.OnError), this.OnError)
                    .On<IEnumerable<ChatMessage>>(nameof(this.OnChatMessages), this.OnChatMessages)
                    .On<RulesetInformation>(nameof(this.OnRulesetInformation), this.OnRulesetInformation)
                    .On<GameInformation>(nameof(this.OnGameInformation), this.OnGameInformation)
                    .On<GameStateInformation>(nameof(this.OnGameStateInformation), this.OnGameStateInformation)
                    .On<LoadingMessage>(nameof(this.OnLoadingMessage), this.OnLoadingMessage)
                    .On<GameUserInformation>(nameof(this.OnGameUserInformation), this.OnGameUserInformation)
                    .StartAsync(gameId);

                await this.JSRuntime.InvokeVoidAsync(Scripts.Game.RESIZE_CANVAS);
            }
            catch(NullReferenceException nullReferenceException)
            {
                Console.WriteLine(nullReferenceException.ToString());
                this.NavigationManager.NavigateTo("/401");
            }
            catch(InvalidOperationException invalidOperationException)
            {
                Console.WriteLine(invalidOperationException.ToString());
                this.NavigationManager.NavigateTo("/404");
            }
            catch(Exception exception)
            {
                Console.WriteLine(exception.ToString());
            }
            finally
            {
                base.StateHasChanged();
            }
        }

        #region Menu
        private async Task ToggleLeftCollapseAsync()
        {
            _isLeftCollapsed = !_isLeftCollapsed;
            base.StateHasChanged();

            await this.JSRuntime.InvokeVoidAsync(Scripts.Game.RESIZE_CANVAS);
        }

        private async Task ToggleRightCollapseAsync()
        {
            _isRightCollapsed = !_isRightCollapsed;
            base.StateHasChanged();

            await this.JSRuntime.InvokeVoidAsync(Scripts.Game.RESIZE_CANVAS);
        }

        private async Task ZoomAsync(WheelEventArgs wheelEventArgs)
        {
            var zoomIncrements = _zoomIncrements.ToList();
            var currentIndex = zoomIncrements.IndexOf(_selectedZoom);
            var zoomIn = wheelEventArgs.DeltaY < 1;
            var nextIndex = Math.Clamp(currentIndex + (zoomIn ? 1 : -1), 0, zoomIncrements.Count - 1);
            var zoom = zoomIncrements[nextIndex];

            await this.SetZoomAsync(zoom);
        }

        private async Task SetZoomAsync(string zoom)
        {
            if (zoom == _selectedZoom)
                return;

            _selectedZoom = zoom;

            await this.JSRuntime.InvokeVoidAsync(Scripts.Game.SET_ZOOM, int.Parse(_selectedZoom));
        }

        private async Task ToggleGridAsync()
        {
            _isGridVisible = !_isGridVisible;
            await this.JSRuntime.InvokeVoidAsync(Scripts.Game.TOGGLE_GRID, _isGridVisible);
        }
        #endregion

        #region Canvas
        private async Task CancelAsync()
        {
            try
            {
                //TODO: Bug/slow down with cancelling inside movement coordinates.
                // TODO: Placing character during combat is broken.
                _clientStateInformation.Clear();
                await this.JSRuntime.InvokeVoidAsync(Scripts.Game.RENDER_CLIENTSTATE, _clientStateInformation);
                // Remove interactive layer from game information/client.
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.ToString());
            }
        }

        private async Task ClickedAsync(GameCoordinate gameCoordinate)
        {
            try
            {
                // TODO: Need to redo visually selected character id in UI. This should be directly tied to _selectedGameCharacter
                // Move this with the toggle character button to abide by same constraints.
                if (_clientStateInformation.Mode == ClientState.None)
                {
                    _clientStateInformation.SelectedGameCharacterId = _gameStateInformation.GameCharacters.FirstOrDefault(c =>
                        c.IsVisible &&
                        c.Id == gameCoordinate.GameCharacterId &&
                       (_rulesetInformation.IsOwner ? true : c.UserId == _clientStateInformation.UserId))?.Id;

                    await this.JSRuntime.InvokeVoidAsync(Scripts.Game.RENDER_CLIENTSTATE, _clientStateInformation);
                       //(_gameStateInformation.GameStateMode == GameStateMode.Combat ? _gameStateInformation.CurrentTurnId == gameCoordinate.GameCharacterId : true))?.Id;
                }

                if (_gameStateInformation.GameStateMode != GameStateMode.Combat)
                    return;

                if (_clientStateInformation.Mode == ClientState.Movement)
                {
                    var selectedCoordinate = _clientStateInformation.Coordinates.FirstOrDefault(c => c.X == gameCoordinate.X && c.Y == gameCoordinate.Y);
                    if (selectedCoordinate == null)
                        return;

                    _clientStateInformation.Clear();
                    await this.JSRuntime.InvokeVoidAsync(Scripts.Game.RENDER_CLIENTSTATE, _clientStateInformation);

                    var gameCharacter = _gameStateInformation.GameCharacters.FirstOrDefault(c => c.Id == _clientStateInformation.SelectedGameCharacterId);
                    if (gameCharacter == null)
                        return;

                    gameCharacter.X = selectedCoordinate.X;
                    gameCharacter.Y = selectedCoordinate.Y;

                    this.OnGameStateInformation(_gameStateInformation);
                    // If the client is selecting movement prevent selecting other characters.
                    await this.GameHubService.SendAsync("CombatPlacement",
                        new RequestCombatPlacement
                        {
                            GameCharacters = new GameCharacter[] { gameCharacter },
                        });
                }
                else if(_clientStateInformation.Mode == ClientState.Attack)
                {
                    var selectedCoordinate = _clientStateInformation.Coordinates.FirstOrDefault(c => c.X == gameCoordinate.X && c.Y == gameCoordinate.Y);
                    if (selectedCoordinate == null)
                        return;

                    var gameCharacter = _gameStateInformation.GameCharacters.FirstOrDefault(c => c.Id == _clientStateInformation.SelectedGameCharacterId);
                    if (gameCharacter == null)
                        return;

                    // If the client is selecting movement prevent selecting other characters.
                    await this.GameHubService.SendAsync("CombatAttack",
                        new RequestCombatAttack
                        {
                            GameCharacterId = gameCharacter.Id,
                            RulesetEntityId = _clientStateInformation.RulesetEntityId.Value,
                            RulesetEntityType = _clientStateInformation.RulesetEntityType,
                            TargetX = selectedCoordinate.X,
                            TargetY = selectedCoordinate.Y,
                        });

                    _clientStateInformation.Clear();
                    await this.JSRuntime.InvokeVoidAsync(Scripts.Game.RENDER_CLIENTSTATE, _clientStateInformation);
                }
                else
                {
                    // TODO: Move context window.
                    //var currentTurn = _gameStateInformation.GameCharacters.FirstOrDefault(c =>
                    //    c.IsVisible &&
                    //    c.Id == gameCoordinate.GameCharacterId &&
                    //   (_rulesetInformation.IsOwner ? true : c.UserId == _userId))?.Id;

                    _clientStateInformation.ShowContextMenu = _clientStateInformation.SelectedGameCharacterId.HasValue;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.ToString());
            }
            finally
            {
                base.StateHasChanged();
            }
        }

        private async Task DeleteAsync(GameCoordinate gameCoordinate)
        {
            try
            {
                if (_gameStateInformation.GameStateMode != GameStateMode.None)
                    return;

                var gameCharacter = _gameStateInformation.GameCharacters.Single(c => c.Id == gameCoordinate.GameCharacterId);
                gameCharacter.IsVisible = false;

                this.OnGameStateInformation(_gameStateInformation);
                // TODO: Replace with update?
                await this.SendPlacementAsync(new GameCharacter[] { gameCharacter });
            }
            catch(Exception exception)
            {
                Console.WriteLine(exception.ToString());
            }
        }

        private async Task MovedAsync(GameCoordinate gameCoordinate)
        {
            try
            {
                var gameCharacter = _gameStateInformation.GameCharacters.Single(c => c.Id == gameCoordinate.GameCharacterId);
                gameCharacter.X = gameCoordinate.X;
                gameCharacter.Y = gameCoordinate.Y;
                gameCharacter.IsVisible = true;

                _clientStateInformation.Clear();
                await this.JSRuntime.InvokeVoidAsync(Scripts.Game.RENDER_CLIENTSTATE, _clientStateInformation);
                this.OnGameStateInformation(_gameStateInformation);

                await this.SendPlacementAsync(new GameCharacter[] { gameCharacter });
            }
            catch(Exception exception)
            {
                Console.WriteLine(exception.ToString());
            }
        }

        private async Task SendPlacementAsync(IEnumerable<GameCharacter> request)
        {
            await this.GameHubService.SendAsync("Placement", 
                new RequestPlacement
                {
                    GameCharacters = request,
                });
        }

        #endregion

        private async Task MoveCharacterAsync()
        {
            try
            {
                // TODO: Validate userid / ownership.
                var gameCharacter = _gameStateInformation.GameCharacters.FirstOrDefault(c => c.Id == _clientStateInformation.SelectedGameCharacterId);
                if (gameCharacter == null)
                    return;

                _clientStateInformation.ShowContextMenu = false;
                //_clientStateInformation.Coordinates = _rulesetInformation.GetMovementCoordinates(_gameInformation, _gameStateInformation, gameCharacter);
                _clientStateInformation.Mode = ClientState.Movement;

                await this.JSRuntime.InvokeVoidAsync(Scripts.Game.RENDER_CLIENTSTATE, _clientStateInformation);

                await this.GameHubService.SendAsync("CombatMove", new { GameCharacterId = gameCharacter.Id });
            }
            catch(Exception exception)
            {
                Console.WriteLine(exception.ToString());
            }
        }

        private async Task UseWeaponAsync(Guid id)
        {
            try
            {
                var gameCharacter = _gameStateInformation.GameCharacters.FirstOrDefault(c => c.Id == _clientStateInformation.SelectedGameCharacterId);
                if (gameCharacter == null)
                    return;

                var weapon = gameCharacter.Weapons.FirstOrDefault(w => w.WeaponId == id);

                _clientStateInformation.ShowContextMenu = false;
                _clientStateInformation.Coordinates = _gameInformation.GetWeaponTargets(_rulesetInformation, _gameStateInformation, gameCharacter, weapon);
                _clientStateInformation.AttackDimensions = weapon.GetAttackDimensions();
                _clientStateInformation.Origin = new Coordinate
                {
                    X = gameCharacter.X,
                    Y = gameCharacter.Y,
                };
                _clientStateInformation.Mode = ClientState.Attack;
                _clientStateInformation.RulesetEntityId = weapon.WeaponId;

                await this.JSRuntime.InvokeVoidAsync(Scripts.Game.RENDER_CLIENTSTATE, _clientStateInformation);
            }
            catch(Exception exception)
            {

            }
        }
   
        private async Task ToggleCombatAsync()
        {
            try
            {
                if (_gameStateInformation.GameStateMode == GameStateMode.None)
                    await this.StartCombatAsync();
                else
                    await this.EndCombatAsync();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.ToString());
            }
        }

        private async Task StartCombatAsync()
        {
            try
            {
                await this.GameHubService.SendAsync("StartCombat", new RequestStartCombat());
            }
            catch (Exception exception)
            {
                Console.Write(exception.ToString());
            }
        }

        private async Task EndCombatAsync()
        {
            try
            {
                await this.GameHubService.SendAsync("EndCombat", new RequestEndCombat());
            }
            catch (Exception exception)
            {
                Console.Write(exception.ToString());
            }
        }

        private async Task EndTurnAsync()
        {
            try
            {
                if (_gameStateInformation.CurrentTurnId != _clientStateInformation.SelectedGameCharacterId)
                    return;

                await this.GameHubService.SendAsync("EndTurn", new RequestEndTurn());
            }
            catch(Exception exception)
            {
                Console.Write(exception.ToString());
            }
        }

        #region Hub

        private void OnError(ErrorMessage errorMessage)
        {
            _isLoading = false;
            _loadingMessage = null;

            if(errorMessage.StatusCode == 401)
                this.NavigationManager.NavigateTo("/401");
        }

        private async void OnChatMessages(IEnumerable<ChatMessage> chatMessages)
        {
            try
            {
                // TODO: When reconnecting to game it will send all chat messages duplicating messages, probably need to change this to return all messages in the game.
                // Since Im restricting the number of messages it might be okay? or I will need to make two different methods to handle loading game/ versus sending messages.
                _chatMessages.AddRange(chatMessages);

                base.StateHasChanged();
                await this.JSRuntime.InvokeVoidAsync(Scripts.Common.SCROLL_TO_BOTTOM, "chatMessages");
            }
            catch(Exception exception)
            {
                Console.WriteLine(exception.ToString());
            }
        }

        private async void OnGameInformation(GameInformation gameInformation)
        {
            try
            {
                _gameInformation = gameInformation;

                await this.JSRuntime.InvokeVoidAsync(Scripts.Game.RENDER_GAME, gameInformation);
                await this.SetZoomAsync("100");
            }
            catch(Exception exception)
            {
                Console.WriteLine(exception.ToString());
            }
            finally
            {
                base.StateHasChanged();
            }
        }

        private async void OnGameStateInformation(GameStateInformation gameStateInformation)
        {
            try
            {
                if(_gameStateInformation.GameStateMode != gameStateInformation.GameStateMode || 
                   _gameStateInformation.CurrentTurnId != gameStateInformation.CurrentTurnId)
                {
                    _clientStateInformation.Clear();
                }

                if (_clientStateInformation.SelectedGameCharacterId.HasValue)
                {
                    var gameCharacterRemoved = gameStateInformation.GameCharacters.FirstOrDefault(c => c.Id == _clientStateInformation.SelectedGameCharacterId);
                    if (gameCharacterRemoved == null)
                    {
                        _clientStateInformation.SelectedGameCharacterId = null;
                        _clientStateInformation.Clear();
                    }
                }

                _gameStateInformation = gameStateInformation;
                _clientStateInformation.Coordinates = gameStateInformation.Coordinates;
                Console.WriteLine(_clientStateInformation.Coordinates.Count());

                // If the game is in combat, auto show context menu when its owned by the user.
                //if(_gameStateInformation.GameStateMode == GameStateMode.Combat)
                //{
                //    var currentGameCharacter = _gameStateInformation.GameCharacters.SingleOrDefault(c => c.Id == gameStateInformation.CurrentTurnId);
                //    if (currentGameCharacter != null && currentGameCharacter.UserId == _userId)
                //        _clientStateInformation.ShowContextMenu = true;

                //    // TODO: Move camera to character for client.
                //}

                await this.JSRuntime.InvokeVoidAsync(Scripts.Game.RENDER_CLIENTSTATE, _clientStateInformation);
                await this.JSRuntime.InvokeVoidAsync(Scripts.Game.RENDER_GAMESTATE, gameStateInformation);
            }
            catch(Exception exception)
            {
                Console.WriteLine(exception.ToString());
            }
            finally
            {
                base.StateHasChanged();
            }
        }

        private void OnRulesetInformation(RulesetInformation rulesetInformation)
        {
            try
            {
                _rulesetInformation = rulesetInformation;
                _clientStateInformation.IsOwner = rulesetInformation.IsOwner;
            }
            catch(Exception exception)
            {
                Console.WriteLine(exception.ToString());
            }
            finally
            {
                base.StateHasChanged();
            }
        }

        private void OnLoadingMessage(LoadingMessage loadingMessage)
        {
            try
            {
                _isLoading = !string.IsNullOrEmpty(loadingMessage.Message);
                _loadingMessage = loadingMessage.Message;
            }
            catch(Exception exception)
            {
                Console.WriteLine(exception);
            }
            finally
            {
                base.StateHasChanged();
            }
        }

        private void OnGameUserInformation(GameUserInformation gameUserInformation)
        {
            try
            {
                _gameUserInformation = gameUserInformation;
            }
            catch(Exception exception)
            {
                Console.WriteLine(exception);
            }
            finally
            {
                base.StateHasChanged();
            }
        }
        #endregion

        #region IDisposable
        public void Dispose()
        {
            try
            {
                this.GameHubService.Dispose();
            }
            catch(Exception exception)
            {
                Console.WriteLine(exception.ToString());
            }
        }
        #endregion
    }
}
