using BlazorInputFile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
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
    public partial class GameCreateView : ComponentBase
    {
        private Game _game;
        private IEnumerable<Ruleset> _rulesets = new List<Ruleset>();
        private EditContextValidator _editContextValidator;
        private bool _isInitialized;
        private bool _isSaving;
        private string _errorMessage;

        [Inject]
        private IRulesetService RulesetService { get; set; }

        [Inject]
        private IGameService GameService { get; set; }

        [Inject]
        private NavigationManager NavigationManager { get; set; }

        [Inject]
        private IJSRuntime JSRuntime { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                _game = Game.Empty;
                _editContextValidator = new EditContextValidator(_game);
            }
            catch (Exception exception)
            {
                _errorMessage = Constants.INITIALIZE_ERROR;
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
                return;

            try
            {
                _rulesets = await this.RulesetService.GetAllAsync();
                _game.RulesetId = _rulesets.FirstOrDefault()?.Id ?? Guid.Empty;

                _isInitialized = true;
                base.StateHasChanged();

                await this.JSRuntime.InvokeVoidAsync(Scripts.Semantic.DROPDOWN);
            }
            catch(Exception exception)
            {
                _errorMessage = Constants.INITIALIZE_ERROR;
            }
            finally
            {
                _isInitialized = true;
                base.StateHasChanged();
            }
        }


        private async Task SubmitAsync()
        {
            try
            {
                var isValid = _editContextValidator.Validate();
                if (!isValid)
                    return;

                _isSaving = true;

                var game = await this.GameService.AddAsync(_game.RulesetId, _game);

                this.NavigationManager.NavigateTo($"/rulesets/{game.RulesetId}/gameview/{game.Id}");
            }
            catch(Exception exception)
            {
                _errorMessage = Constants.SAVING_ERROR;
            }
            finally
            {
                _isSaving = false;
            }
        }
    }
}
