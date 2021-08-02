using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Simucraft.Client.Common;
using Simucraft.Client.Models;
using Simucraft.Client.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Simucraft.Client.Pages
{
    [Authorize]
    public partial class GameList : ComponentBase
    {
        private IEnumerable<Game> _games = new List<Game>();
        private IEnumerable<Game> _invitedGames = new List<Game>();

        private bool _isInitialized;
        private Guid _selectedGameId;
        private string _errorMessage;

        [Inject]
        private NavigationManager NavigationManager { get; set; }

        [Inject]
        private IJSRuntime JSRuntime { get; set; }

        [Inject]
        private IGameService GameService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                _games = await this.GameService.GetAllAsync();
                _invitedGames = await this.GameService.GetInvitedAsync();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.ToString());
                _errorMessage = Constants.INITIALIZE_ERROR;
            }
            finally
            {
                _isInitialized = true;
            }
        }

        private async Task PromptDeleteAsync(Guid gameId)
        {
            _errorMessage = null;
            await this.JSRuntime.InvokeVoidAsync(Scripts.Semantic.SHOW_DELETE_CONFIRMATION);
            _selectedGameId = gameId;
        }

        private async Task DeleteAsync()
        {
            try
            {
                var gameId = _selectedGameId;
                _games = _games.Where(g => g.Id != gameId);
                await this.GameService.DeleteAsync(gameId);
            }
            catch (Exception exception)
            {
                _errorMessage = Constants.DELETE_ERROR;
            }
        }
    }
}
