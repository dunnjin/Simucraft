using Microsoft.AspNetCore.Components;

namespace Simucraft.Client.Components
{
    public partial class DeleteConfirmation : ComponentBase
    {
        [Parameter]
        public string ItemType { get; set; }

        [Parameter]
        public EventCallback ConfirmClicked { get; set; }
    }
}
