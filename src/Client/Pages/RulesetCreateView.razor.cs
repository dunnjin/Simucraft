using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Simucraft.Client.Common;
using Simucraft.Client.Components;
using Simucraft.Client.Models;
using Simucraft.Client.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Simucraft.Client.Pages
{
    [Authorize]
    public partial class RulesetCreateView : ComponentBase
    {
        private EditContextValidator _editContextValidator;
        private Ruleset _ruleset;
        private bool _isSaving;
        private string _errorMessage;

        [Inject]
        private IRulesetService RulesetService { get; set; }

        [Inject]
        private IJSRuntime JSRuntime { get; set; }

        [Inject]
        private NavigationManager NavigationManager { get; set; }

        protected override void OnInitialized()
        {
            _ruleset = Ruleset.Empty;
            _ruleset.MovementOffset = 4;

            _editContextValidator = new EditContextValidator(_ruleset);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
                return;

            await this.JSRuntime.InvokeVoidAsync(Scripts.Semantic.DROPDOWN);
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

                var ruleset = await this.RulesetService.AddAsync(_ruleset);

                this.NavigationManager.NavigateTo($"/rulesetview/{ruleset.Id}");
            }
            catch (SubscriptionException subscriptionException)
            {
                _errorMessage = subscriptionException.Message;
            }
            catch (Exception exception)
            {
                _errorMessage = "An error occured while saving the Ruleset, please try again later";
            }
            finally
            {
                _isSaving = false;
            }
        }
    }
}
