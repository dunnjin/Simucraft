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
    public partial class RulesetBasic : ComponentBase
    {
        private EditContextValidator _editContextValidator;
        private Ruleset _ruleset;
        private bool _isSaving;
        private string _errorMessage;

        [Inject]
        private IRulesetService RulesetService { get; set; }

        [Inject]
        private IJSRuntime JSRuntime { get; set; }

        [Parameter]
        public Ruleset Ruleset { get; set; }

        [Parameter]
        public EventCallback OnSaved { get; set; }

        protected override void OnInitialized()
        {
            _ruleset = this.Ruleset;

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

                await this.RulesetService.UpdateAsync(_ruleset);
                await this.OnSaved.InvokeAsync(null);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.ToString());
                _errorMessage = Constants.SAVING_ERROR;
            }
            finally
            {
                _isSaving = false;
            }
        }
    }
}
