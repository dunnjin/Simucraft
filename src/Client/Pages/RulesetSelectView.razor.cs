using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Simucraft.Client.Common;
using Simucraft.Client.Services;
using System;
using System.Threading.Tasks;

namespace Simucraft.Client.Pages
{
    [Authorize]
    public partial class RulesetSelectView : ComponentBase
    {
        private string _errorMessage;
        private bool _isLoading;

        [Inject]
        private NavigationManager NavigationManager { get; set; }

        [Inject]
        private IRulesetService RulesetService { get; set; }

        private async Task ImportAsync(Guid templateId)
        {
            try
            {
                if (_isLoading)
                    return;

                _errorMessage = null;
                _isLoading = true;

                var ruleset = await this.RulesetService.ImportAsync(templateId);
                this.NavigationManager.NavigateTo($"/rulesetview/{ruleset.Id}");
            }
            catch(SubscriptionException subscriptionException)
            {
                _errorMessage = subscriptionException.Message;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                _errorMessage = Constants.SAVING_ERROR;
            }
            finally
            {
                _isLoading = false;
            }
        }
    }
}
