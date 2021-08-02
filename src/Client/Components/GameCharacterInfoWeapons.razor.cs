using Microsoft.AspNetCore.Components;
using Simucraft.Client.Models;
using Simucraft.Client.Services;
using System;
using System.Threading.Tasks;

namespace Simucraft.Client.Components
{
    public partial class GameCharacterInfoWeapons
    {
        [Parameter]
        public GameCharacter GameCharacter { get; set; }

        [Parameter]
        public GameHubService GameHubService { get; set; }

        private async Task WeaponSelectedAsync(Weapon weapon)
        {
            try
            {
                var gameCharacterId = this.GameCharacter?.Id;
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
