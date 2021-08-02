using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Simucraft.Client.Common;
using Simucraft.Client.Services;
using System;
using System.Threading.Tasks;

namespace Simucraft.Client.Pages
{
    [Authorize]
    public partial class GameInviteView : ComponentBase
    {
        private string _errorMessage;

        [Parameter]
        public string RulesetId { get; set; }

        [Parameter]
        public string GameId { get; set; }

        [Parameter]
        public string InviteId { get; set; }

        [Inject]
        private NavigationManager NavigationManager { get; set; }

        [Inject]
        private IGameService GameService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                if (!Guid.TryParse(this.RulesetId, out var rulesetId))
                    throw new NullReferenceException(nameof(this.RulesetId));

                if (!Guid.TryParse(this.GameId, out var gameId))
                    throw new NullReferenceException(nameof(this.GameId));

                if (string.IsNullOrEmpty(this.InviteId))
                    throw new NullReferenceException(nameof(this.InviteId));

                await this.GameService.AcceptInviteAsync(rulesetId, gameId, this.InviteId);

                this.NavigationManager.NavigateTo($"/rulesets/{rulesetId}/gameview/{gameId}");
            }
            catch(NullReferenceException)
            {
                this.NavigationManager.NavigateTo("/404");
            }
            catch(InvalidOperationException)
            {
                _errorMessage = "Invitation has expired or is no longer available.";
            }
            catch(Exception exception)
            {
                _errorMessage = "Unable to accept Invite at this time, please try again later.";
            }
        }
    }
}
