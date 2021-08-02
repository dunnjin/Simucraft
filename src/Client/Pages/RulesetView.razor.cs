using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Simucraft.Client.Models;
using Simucraft.Client.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Simucraft.Client.Pages
{
    [Authorize]
    public partial class RulesetView : ComponentBase
    {
        private Ruleset _ruleset;
        private RulesetViewType _selectedRulesetViewType;
        private ICollection<RulesetViewType> _initializedRulesetViewTypes = new List<RulesetViewType>();

        private string _errorMessage;

        [Inject]
        private IRulesetService RulesetService { get; set; }

        [Inject]
        private NavigationManager NavigationManager { get; set; }

        [Inject]
        private ICharacterService CharacterService { get; set; }

        [Inject]
        private IWeaponService WeaponService { get; set; }

        [Inject]
        private IEquipmentService EquipmentService { get; set; }

        [Inject]
        private ISpellService SpellService { get; set; }

        [Inject]
        private ISkillService SkillService { get; set; }

        [Parameter]
        public string Id { get; set; }

        protected override async Task OnInitializedAsync()
        {
            _ruleset = Ruleset.Empty;

            try
            {
                if (!Guid.TryParse(this.Id, out var rulesetId))
                    throw new InvalidOperationException("Invalid Id.");

                _ruleset = await this.RulesetService.GetByIdAsync(rulesetId);

                _initializedRulesetViewTypes.Add(RulesetViewType.Basic);

                _ruleset.Skills = (await this.SkillService
                    .GetAllByRulesetIdAsync(rulesetId))
                    .ToList();

                _initializedRulesetViewTypes.Add(RulesetViewType.Skills);

                _ruleset.Weapons = (await this.WeaponService
                    .GetAllByRulesetIdAsync(rulesetId))
                    .ToList();

                _initializedRulesetViewTypes.Add(RulesetViewType.Weapons);

                _ruleset.Equipment = (await this.EquipmentService
                    .GetAllByRulesetIdAsync(rulesetId))
                    .ToList();

                _initializedRulesetViewTypes.Add(RulesetViewType.Equipment);

                _ruleset.Spells = (await this.SpellService
                    .GetAllByRulesetIdAsync(rulesetId))
                    .ToList();

                _initializedRulesetViewTypes.Add(RulesetViewType.Spells);

                _ruleset.Characters = (await this.CharacterService
                    .GetAllByRulesetIdAsync(rulesetId))
                    .ToList();

                _initializedRulesetViewTypes.Add(RulesetViewType.Characters);


            }
            catch (InvalidOperationException)
            {
                this.NavigationManager.NavigateTo("/404");
            }
            catch (Exception exception)
            {
                _errorMessage = "An error ocurred while retrieving the Ruleset, please try again later.";
            }
            finally
            {
            }
        }
    }
}
