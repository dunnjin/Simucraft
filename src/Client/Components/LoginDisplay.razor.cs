using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System.Threading.Tasks;

namespace Simucraft.Client.Components
{
    public partial class LoginDisplay : ComponentBase
    {
        [Inject]
        private NavigationManager NavigationManager { get; set; }

        [Inject]
        private SignOutSessionStateManager SignOutSessionStateManager { get; set; }

        private async Task SignOutAsync()
        {
            await this.SignOutSessionStateManager.SetSignOutState();
            this.NavigationManager.NavigateTo("/authentication/logout");
        }
    }
}
