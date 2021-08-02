using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Simucraft.Client.Common;
using Simucraft.Client.Core;
using Simucraft.Client.Models;
using Simucraft.Client.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Simucraft.Client.Components
{
    public partial class GameCharacterInfo : ComponentBase
    {
        private bool _isEditGameCharacter;
        private GameCharacterInfoView _gameCharacterInfoView;

        [Parameter]
        public ClientStateInformation ClientState { get; set; }

        [Parameter]
        public GameStateInformation GameState { get; set; }

        [Parameter]
        public GameInformation Game { get; set; }

        [Parameter]
        public RulesetInformation Ruleset { get; set; }

        [Parameter]
        public GameHubService GameHubService { get; set; }

        [Inject]
        private IJSRuntime JSRuntime { get; set; }

        public GameCharacter SelectedGameCharacter => this.GameState.GameCharacters.FirstOrDefault(c => c.Id == this.ClientState.SelectedGameCharacterId);

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await this.JSRuntime.InvokeVoidAsync(Scripts.Game.RESIZE_UI);
            if (!firstRender)
                return;

        }

        private async Task SkillSelectedAsync(GameCharacterSkill gameCharacterSkill)
        {
            try
            {
                var gameCharacterId = this.ClientState.SelectedGameCharacterId;
                if (!gameCharacterId.HasValue  || string.IsNullOrEmpty(gameCharacterSkill.Name) || string.IsNullOrEmpty(gameCharacterSkill.Expression))
                    return;

                await this.GameHubService.SendAsync("RollGameCharacter",
                    new RequestRollGameCharacter
                    {
                        GameCharacterId = gameCharacterId.Value,
                        RulesetEntityId = gameCharacterSkill.SkillId,
                        RulesetEntityType = RulesetEntityType.Skill,
                    });
            }
            catch(Exception exception)
            {
                Console.Write(exception.ToString());
            }
        }

        private async Task WeaponSelectedAsync(Weapon weapon)
        {
            try
            {
                var gameCharacterId = this.ClientState.SelectedGameCharacterId;
                if (!gameCharacterId.HasValue)
                    return;

                await this.GameHubService.SendAsync("RollGameCharacter",
                    new RequestRollGameCharacter
                    {
                        GameCharacterId = gameCharacterId.Value,
                        RulesetEntityId = weapon.WeaponId,
                        RulesetEntityType = RulesetEntityType.Weapon,
                    });
            }
            catch (Exception exception)
            {
                Console.Write(exception.ToString());
            }
        }
    }
}
