using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Simucraft.Client.Models;
using Simucraft.Client.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Simucraft.Client.Components
{
    public partial class GameChatBar
    {
        private ChatMessage _chatMessage;

        [Parameter]
        public RulesetInformation Ruleset { get; set; }

        [Parameter]
        public GameHubService GameHubService { get; set; }

        [Parameter]
        public ClientStateInformation ClientState { get; set; }

        [Parameter]
        public IEnumerable<ChatMessage> ChatMessages { get; set; }

        [Inject]
        private IJSRuntime JSRuntime { get; set; }

        protected override void OnInitialized()
        {
            _chatMessage = ChatMessage.Empty;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await this.JSRuntime.InvokeVoidAsync("semanticHelpers.tooltip");
        }

        private async Task SendChatMessageAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(_chatMessage.Message))
                    return;

                await this.GameHubService.SendAsync("SendChatMessage", _chatMessage);
                _chatMessage.Message = null;
            }
            catch (Exception exception)
            {

            }
        }

    }
}
