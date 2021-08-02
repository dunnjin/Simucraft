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
    public partial class RulesetList : ComponentBase
    {
        private IEnumerable<Ruleset> _rulesets = new List<Ruleset>();
        private bool _isInitialized;
        private Guid _selectedRulesetId;
        private string _errorMessage;

        [Inject]
        private IRulesetService RulesetService { get; set; }

        [Inject]
        private NavigationManager NavigationManager { get; set; }

        [Inject]
        private IJSRuntime JSRuntime { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                _rulesets = await this.RulesetService.GetAllAsync();
            }
            catch(Exception exception)
            {
                Console.WriteLine(exception.ToString());
                _errorMessage = "Unable to retreive Rulesets at this time, please try again later";
            }
            finally
            {
                _isInitialized = true;
            }
        }

        private async Task PromptDeleteRulesetAsync(Guid rulesetId)
        {
            _errorMessage = null;
            await this.JSRuntime.InvokeVoidAsync(Scripts.Semantic.SHOW_DELETE_CONFIRMATION);
            _selectedRulesetId = rulesetId;

        }

        private async Task DeleteRulesetAsync()
        {
            try
            {
                _errorMessage = "Deleting Ruleset is not supported at this time.";
                return;

                var rulesetId = _selectedRulesetId;
                _rulesets = _rulesets.Where(r => r.Id != rulesetId);
                await this.RulesetService.DeleteAsync(rulesetId);
            }
            catch(Exception exception)
            {
                _errorMessage = "Unable to delete at this time, please try again later.";
            }
        }
    }
}
