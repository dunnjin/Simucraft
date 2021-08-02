using BlazorInputFile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Simucraft.Client.Common;
using Simucraft.Client.Components;
using Simucraft.Client.Models;
using Simucraft.Client.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Simucraft.Client.Pages
{
    [Authorize]
    public partial class MapView : ComponentBase
    {
        private Map _map;
        private EditContextValidator _editContextValidator;
        private bool _isLoadingImage;
        private bool _isSaving;
        private bool _isGridVisible;
        private bool _isInitialized;
        private CollisionType _collisionType;
        private MapViewOption _mapViewOption;
        private MapMenuOption _mapMenuOption;
        private string _selectedZoom;
        private string _errorMessage;
        private IList<string> _zoomIncrements = new List<string>();
        private IEnumerable<Character> _characters = new List<Character>();
        private Guid? _selectedCharacterId;

        [Inject]
        private IJSRuntime JSRuntime { get; set; }

        [Inject]
        private IMapService MapService { get; set; }

        [Inject]
        private ICharacterService CharacterService { get; set; }

        [Inject]
        private NavigationManager NavigationManager { get; set; }

        [Parameter]
        public string RulesetId { get; set; }

        [Parameter]
        public string Id { get; set; }

        protected override void OnInitialized() 
        {
            try
            {
                _isGridVisible = true;
                _collisionType = CollisionType.Bottom | CollisionType.Left | CollisionType.Right | CollisionType.Top;
                _map = Map.Empty;
                _zoomIncrements = new List<string> { "25", "50", "75", "100", "125", "150", "175", "200", "250", "300", "400" };
                _selectedZoom = "100";
                _mapMenuOption = MapMenuOption.Characters;
                _editContextValidator = new EditContextValidator(_map);
            }
            catch(InvalidOperationException)
            {
                this.NavigationManager.NavigateTo("/404");
            }
            catch(Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
                return;

            try
            {
                if (!Guid.TryParse(this.RulesetId, out var rulesetId))
                    throw new InvalidOperationException("Invalid ruleset id.");

                if (!Guid.TryParse(this.Id, out var id))
                    throw new InvalidOperationException(nameof(this.Id));

                _map = await this.MapService.GetByIdAsync(id);
                _editContextValidator = new EditContextValidator(_map);

                await this.JSRuntime.InvokeVoidAsync(Scripts.Map.INITIALIZE, _map);
                await this.JSRuntime.InvokeVoidAsync(Scripts.Map.SET_MAP_MENU_OPTION, _mapMenuOption);

                _isInitialized = true;
                base.StateHasChanged();

                _characters = await this.CharacterService.GetAllByRulesetIdAsync(rulesetId);

                await this.JSRuntime.InvokeVoidAsync(Scripts.Semantic.DROPDOWN);

                // Create existing characters in map.
                foreach (var mapCharacter in _map.MapCharacters.Where(mc => _characters.Any(c => mc.CharacterId == c.Id)))
                {
                    var character = _characters.Single(c => c.Id == mapCharacter.CharacterId);
                    await this.JSRuntime.InvokeVoidAsync(
                        Scripts.Map.ADD_CHARACTER,
                        new
                        {
                            Id = Guid.NewGuid(),
                            CharacterId = character.Id,
                            character.Width,
                            character.Height,
                            mapCharacter.X,
                            mapCharacter.Y,
                            character.ImageName,
                            character.ImageUrl,
                        });
                }
            }
            catch(InvalidOperationException)
            {
                this.NavigationManager.NavigateTo("/404");
            }
            catch(Exception exception)
            {
                Console.WriteLine(exception.ToString());
            }
            finally
            {
                _isInitialized = true;
                base.StateHasChanged();
            }
        }

        private async Task ToggleGridAsync()
        {
            _isGridVisible = !_isGridVisible;
            await this.JSRuntime.InvokeVoidAsync(Scripts.Map.TOGGLE_GRID, _isGridVisible);
        }

        private async Task SetCollisionAsync()
        {
            _mapMenuOption = MapMenuOption.Collision;
            _selectedCharacterId = null;
            await this.JSRuntime.InvokeVoidAsync(Scripts.Map.SET_MAP_MENU_OPTION, _mapMenuOption);
        }

        private async Task SubmitAsync()
        {
            try
            {
                _errorMessage = null;

                var isValidated = _editContextValidator.Validate();
                if (!isValidated)
                    return;

                _isSaving = true;

                // Get data from map script.
                var mapData = await this.JSRuntime.InvokeAsync<Map>(Scripts.Map.GET_MAP);
                _map.Width = mapData.Width;
                _map.Height = mapData.Height;
                _map.CollisionTiles = mapData.CollisionTiles;
                _map.MapCharacters = mapData.MapCharacters;

                // TODO: Validate map width/height.

                await this.MapService.UpdateAsync(Guid.Parse(this.RulesetId), _map);
            }
            catch(Exception exception)
            {
                Console.WriteLine(exception.ToString());
                _errorMessage = "An error occurred while saving, please try again later.";
            }
            finally
            {
                _isSaving = false;
            }
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

            await this.JSRuntime.InvokeVoidAsync(Scripts.Map.SET_ZOOM, int.Parse(_selectedZoom));
        }

        private async Task ToggleCollisionTypeAsync(CollisionType collisionType)
        {
            var type = _collisionType ^ collisionType;
            await this.SetCollisionTypeAsync(type);
        }

        public async Task SetCollisionTypeAsync(CollisionType collisionType)
        {
            _collisionType = collisionType;
            await this.JSRuntime.InvokeVoidAsync(Scripts.Map.SET_COLLISION_TYPE, (int)_collisionType); 
        }

        private async Task ToggleCharacterSelectionAsync(Guid? id)
        {
            try
            {
                _selectedCharacterId = _selectedCharacterId == id ? null : id;
                _mapMenuOption = MapMenuOption.Characters;

                await this.JSRuntime.InvokeVoidAsync(Scripts.Map.SET_MAP_MENU_OPTION, _mapMenuOption);

                var character = _characters.SingleOrDefault(c => c.Id == _selectedCharacterId);

                await this.JSRuntime.InvokeVoidAsync(
                    Scripts.Map.SELECT_CHARACTER,
                    character == null ? null :
                    new
                    {
                        Id = Guid.NewGuid(),
                        CharacterId = character.Id,
                        character.Width,
                        character.Height,
                        X = 0,
                        Y = 0,
                        character.ImageName,
                        character.ImageUrl,
                    });
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

        private async void SetMapViewOption(MapViewOption mapViewOption)
        {
            if (string.IsNullOrEmpty(_map.ImageName))
                return;

            _mapViewOption = mapViewOption;

            if(mapViewOption == MapViewOption.Characters)
            {
                await this.JSRuntime.InvokeVoidAsync("map.resizeCharacterList");
            }
        }

        private async Task RemoveSelectedCharacter(MouseEventArgs mouseEventArgs)
        {
            try
            {
                // Remove the selected character.
                if (mouseEventArgs.Button != 2 || _selectedCharacterId == null || _mapMenuOption != MapMenuOption.Characters)
                    return;

                await this.ToggleCharacterSelectionAsync(null);
            }
            catch(Exception exception)
            {
                Console.WriteLine(exception.ToString());
            }
        }
    }
}
