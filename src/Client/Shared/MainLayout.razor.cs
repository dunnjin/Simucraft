using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Simucraft.Client.Common;
using System;
using System.Threading.Tasks;

namespace Simucraft.Client.Shared
{
    public partial class MainLayout : LayoutComponentBase
    {
        private string _version;

        [Inject]
        private IJSRuntime JSRuntime { get; set; }

        protected override Task OnInitializedAsync()
        {
            try
            {
                _version = this
                    .GetType().Assembly
                    .GetName().Version
                    .ToString();

                _version = "0.1.0 -alpha";
            }
            catch(Exception exception)
            {
            }

            return base.OnInitializedAsync();
        }

        [JSInvokable("CreateGuid")]
        public static Task<Guid> CreateGuid() => Task.FromResult(Guid.NewGuid());

        private async Task ToggleAsync() =>
            await this.JSRuntime.InvokeVoidAsync("semanticHelpers.toggleSidebar");
    }
}
