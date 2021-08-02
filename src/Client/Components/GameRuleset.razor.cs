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
    public partial class GameRuleset : ComponentBase
    {
        private string _searchInput;

        private Guid? _selectedMapId;
        private Guid? _selectedCharacterId;
        private Guid? _selectedWeaponId;

        [Parameter]
        public RulesetInformation Ruleset { get; set; }

        [Parameter]
        public GameUserInformation UserInformation { get; set; }

        [Parameter]
        public GameStateInformation GameState { get; set; }

        [Parameter]
        public GameInformation Game { get; set; }

        [Parameter]
        public GameHubService GameHubService { get; set; }

        [Parameter]
        public ClientStateInformation ClientState { get; set; }

        [Inject]
        private IJSRuntime JSRuntime { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await this.JSRuntime.InvokeVoidAsync(Scripts.Semantic.DROPDOWN);

            if (!firstRender)
                return;

            await this.JSRuntime.InvokeVoidAsync(Scripts.Game.RESIZE_UI);
        }

        private async Task LoadMapAsync()
        {
            try
            {
                if (!_selectedMapId.HasValue)
                    return;

                var request = new RequestLoadMap
                {
                    MapId = _selectedMapId.Value,
                };

                await this.GameHubService.SendAsync("LoadMap", request);
            }
            catch(Exception exception)
            {
                Console.WriteLine(exception.ToString());
            }
        }

        private async Task AddCharacterAsync()
        {
            try
            {
                if (!_selectedCharacterId.HasValue)
                    return;

                var request = new RequestAddCharacter
                {
                     CharacterId = _selectedCharacterId.Value,
                };

                await this.GameHubService.SendAsync("AddCharacter", request);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.ToString());
            }
        }

        private async Task SendWeaponAsync(Guid characterId, Guid weaponId)
        {
            try
            {
                var gameCharacter = this.GameState.GameCharacters.FirstOrDefault(c => c.Id == characterId);
                if (gameCharacter == null)
                    return;

                var weapon = this.Ruleset.Weapons.FirstOrDefault(w => w.Id == weaponId);
                if (weapon == null)
                    return;

                if (gameCharacter.Weapons.Any(w => w.WeaponId == weapon.Id))
                    return;

                var newWeapon = weapon.Copy();
                newWeapon.WeaponId = newWeapon.Id;
                gameCharacter.Weapons.Add(newWeapon);

                await this.GameHubService.SendAsync("UpdateGameCharacter", new { gameCharacter });
            }
            catch (Exception exception)
            {
                Console.Write(exception.ToString());
            }
        }
    }
}
