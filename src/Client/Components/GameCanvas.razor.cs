using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Simucraft.Client.Common;
using Simucraft.Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Simucraft.Client.Components
{
    public partial class GameCanvas : ComponentBase
    {
        private double _previousX;
        private double _previousY;

        private double _previousCoordinateX;
        private double _previousCoordinateY;

        private bool _isClicked;
        private long _button;

        [Parameter]
        public GameInformation Game { get; set; }

        [Parameter]
        public GameStateInformation GameState { get; set; }

        [Parameter]
        public ClientStateInformation ClientState { get; set; }

        [Parameter]
        public EventCallback<WheelEventArgs> OnZoom { get; set; }

        [Parameter]
        public EventCallback<GameCoordinate> OnClicked { get; set; }

        [Parameter]
        public EventCallback<GameCoordinate> OnMoved { get; set; }

        [Parameter]
        public EventCallback OnCancel { get; set; }

        [Parameter]
        public EventCallback<GameCoordinate> OnDelete { get; set; }

        [Inject]
        private IJSRuntime JSRuntime { get; set; }

        private async Task ZoomAsync(WheelEventArgs wheelEventArgs)
        {
            if (!this.Game.MapId.HasValue)
                return;

            await this.OnZoom.InvokeAsync(wheelEventArgs);
        }

        private async Task MouseClickAsync(MouseEventArgs mouseEventArgs)
        {
            if (!this.Game.MapId.HasValue)
                return;

            if (_button != 1)
                return;

            if (this.GameState.GameStateMode != GameStateMode.Combat)
                return;

            // Clicked.
            var mouseCoordinates = await this.JSRuntime.InvokeAsync<Coordinate>(Scripts.Game.GET_COORDINATES);
            if (mouseCoordinates.X < 0 || mouseCoordinates.X >= this.Game.Width || mouseCoordinates.Y < 0 || mouseCoordinates.Y >= this.Game.Height)
                return;

            // Placement.
            var placementValue = await this.JSRuntime.InvokeAsync<string>("game.getPlacementCharacterId");
            if (Guid.TryParse(placementValue, out var placementId))
            {
                await this.OnMoved.InvokeAsync(
                   new GameCoordinate
                   {
                       X = mouseCoordinates.X,
                       Y = mouseCoordinates.Y,
                       GameCharacterId = placementId,
                   });

                return;
            }

            // Generic click.
            await this.OnClicked.InvokeAsync(
                new GameCoordinate
                {
                    X = mouseCoordinates.X,
                    Y = mouseCoordinates.Y,
                    GameCharacterId = this.GameState.GameCharacters.FirstOrDefault(c => c.X == mouseCoordinates.X && c.Y == mouseCoordinates.Y)?.Id,
                });
        }

        private void MouseDown(MouseEventArgs mouseEventArgs)
        {
            if (!this.Game.MapId.HasValue)
                return;

            _previousX = mouseEventArgs.ClientX;
            _previousY = mouseEventArgs.ClientY;
            _button = mouseEventArgs.Buttons;
        }

        private async Task MouseUpAsync(MouseEventArgs mouseEventArgs)
        {
            if (!this.Game.MapId.HasValue)
                return;

            // Cancel.
            if (_button == 2 && _previousX == mouseEventArgs.ClientX && _previousY == mouseEventArgs.ClientY)
                await this.OnCancel.InvokeAsync(null);

            // Left mouse button.
            var supportedKeyInputs = new long[] { 1, 3, 5 };
            if (!supportedKeyInputs.Contains(_button))
                return;

            if (this.GameState.GameStateMode != GameStateMode.None)
                return;

            // Dragged.
            if (_previousX != mouseEventArgs.ClientX || _previousY != mouseEventArgs.ClientY)
            {
                var draggedValue = await this.JSRuntime.InvokeAsync<GameCharacter>(Scripts.Game.GET_DRAGGED_CHARACTER);
                if (draggedValue != null)
                {
                    await this.OnMoved.InvokeAsync(
                        new GameCoordinate
                        {
                            X = draggedValue.X,
                            Y = draggedValue.Y,
                            GameCharacterId = draggedValue.Id,
                        });
                }

                return;
            }

            // Clicked.
            var mouseCoordinates = await this.JSRuntime.InvokeAsync<Coordinate>(Scripts.Game.GET_COORDINATES);
            if (mouseCoordinates.X < 0 || mouseCoordinates.X >= this.Game.Width || mouseCoordinates.Y < 0 || mouseCoordinates.Y >= this.Game.Height)
                return;

            // Deleted.
            var deletedValue = await this.JSRuntime.InvokeAsync<string>(Scripts.Game.GET_DELETED_CHARACTER_ID);
            if (Guid.TryParse(deletedValue, out var gameCharacterId))
            {
                await this.OnDelete.InvokeAsync(
                    new GameCoordinate
                    {
                        GameCharacterId = gameCharacterId,
                        X = mouseCoordinates.X,
                        Y = mouseCoordinates.Y,
                    });

                return;
            }

            // Placement.
            var placementValue = await this.JSRuntime.InvokeAsync<string>("game.getPlacementCharacterId");
            if (Guid.TryParse(placementValue, out var placementId))
            {
                await this.OnMoved.InvokeAsync(
                   new GameCoordinate
                   {
                       X = mouseCoordinates.X,
                       Y = mouseCoordinates.Y,
                       GameCharacterId = placementId,
                   });

                return;
            }

            // Generic click.
            await this.OnClicked.InvokeAsync(
                new GameCoordinate
                {
                    X = mouseCoordinates.X,
                    Y = mouseCoordinates.Y,
                    GameCharacterId = this.GameState.GameCharacters.FirstOrDefault(c => c.X == mouseCoordinates.X && c.Y == mouseCoordinates.Y)?.Id,
                });
        }
    }
}
