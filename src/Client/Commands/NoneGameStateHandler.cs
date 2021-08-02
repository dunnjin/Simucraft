using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Simucraft.Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Simucraft.Client.Commands
{
    public class NoneGameStateHandler : IGameStateHandler
    {
        private readonly IJSRuntime _jsRuntime;

        public NoneGameStateHandler(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public ClientState ClientState => ClientState.None;

        public async Task HandleAsync(Game game, Coordinate coordinate)
        {
            if (coordinate.X < 0 || coordinate.X >= game.Width || coordinate.Y < 0 || coordinate.Y >= game.Height)
                throw new OperationCanceledException();
        }
    }
}
