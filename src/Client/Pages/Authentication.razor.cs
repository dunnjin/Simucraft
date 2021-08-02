using Microsoft.AspNetCore.Components;

namespace Simucraft.Client.Pages
{
    public partial class Authentication : ComponentBase
    {
        [Parameter]
        public string Action { get; set; }
    }
}
