using Microsoft.AspNetCore.Components;
using Simucraft.Client.Models;
using Simucraft.Client.Services;
using System;
using System.Threading.Tasks;

namespace Simucraft.Client.Components
{
    public partial class GameCharacterInfoStats
    {
        [Parameter]
        public GameCharacter GameCharacter { get; set; }

        [Parameter]
        public GameHubService GameHubService { get; set; }

        private async Task SkillSelectedAsync(GameCharacterSkill gameCharacterSkill)
        {
            try
            {
                var gameCharacterId = this.GameCharacter?.Id;
                if (!gameCharacterId.HasValue || string.IsNullOrEmpty(gameCharacterSkill.Name) || string.IsNullOrEmpty(gameCharacterSkill.Expression))
                    return;

                await this.GameHubService.SendAsync("RollGameCharacter",
                    new RequestRollGameCharacter
                    {
                        GameCharacterId = gameCharacterId.Value,
                        RulesetEntityId = gameCharacterSkill.SkillId,
                        RulesetEntityType = RulesetEntityType.Skill,
                    });
            }
            catch (Exception exception)
            {
                Console.Write(exception.ToString());
            }
        }

    }
}
