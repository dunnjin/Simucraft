using Microsoft.AspNetCore.Components;
using System;

namespace Simucraft.Client.Components
{
    public partial class RedirectToLogin : ComponentBase
    {
        [Inject]
        private NavigationManager NavigationManager { get; set; }

        protected override void OnInitialized()
        {
            this.NavigationManager.NavigateTo($"/authentication/login?returnUrl={Uri.EscapeDataString(this.NavigationManager.Uri)}");
        }
    }
}
