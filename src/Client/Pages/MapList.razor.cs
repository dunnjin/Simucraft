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
    public partial class MapList : ComponentBase
    {
        private IEnumerable<Map> _maps = new List<Map>();
        private bool _isInitialized;
        private Guid _selectedMapId;
        private string _errorMessage;

        [Inject]
        private NavigationManager NavigationManager { get; set; }

        [Inject]
        private IJSRuntime JSRuntime { get; set; }

        [Inject]
        private IMapService MapService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                _maps = await this.MapService.GetAllAsync();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.ToString());
                _errorMessage = "Unable to retrieve Maps at this time, please try again later.";
            }
            finally
            {
                _isInitialized = true;
            }
        }

        private async Task PromptDeleteMapAsync(Guid mapId)
        {
            _errorMessage = null;
            await this.JSRuntime.InvokeVoidAsync(Scripts.Semantic.SHOW_DELETE_CONFIRMATION);
            _selectedMapId = mapId;
        }

        private async Task DeleteMapAsync()
        {
            try
            {
                var mapId = _selectedMapId;
                _maps = _maps.Where(m => m.Id != mapId);
                await this.MapService.DeleteAsync(mapId);
            }
            catch (Exception exception)
            {
                _errorMessage = "Unable to delete at this time, please try again later.";
            }
        }
    }
}
