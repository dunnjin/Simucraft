using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Simucraft.Client.Common;
using Simucraft.Client.Models;
using Simucraft.Client.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Simucraft.Client.Components
{
    public partial class RulesetWeapons : ComponentBase
    {
        private string _errorMessage;

        private bool _isListView;
        private bool _isSaving;

        private EditContextValidator _editContextValidator;

        private Weapon _weapon;

        private Guid _weaponId;

        [Inject]
        private IJSRuntime  JSRuntime { get; set; }

        [Inject]
        private IWeaponService WeaponService { get; set; }

        [Parameter]
        public Ruleset Ruleset { get; set; }

        [Parameter]
        public EventCallback OnSaved { get; set; }

        protected override void OnInitialized()
        {
            _isListView = true;

            _weapon = Weapon.Empty;
            _editContextValidator = new EditContextValidator(_weapon);

            base.OnInitialized();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            try
            {
                await this.JSRuntime.InvokeVoidAsync(Scripts.Semantic.DROPDOWN);
            }
            catch(Exception exception)
            {
                Console.WriteLine(exception.ToString());
            }
        }

        private void NavigateToCreate(Guid? id)
        {
            _weapon = id.HasValue
                ? this.Ruleset.Weapons.Single(r => r.Id == id).Copy()
                : Weapon.Empty;

            _editContextValidator = new EditContextValidator(_weapon);
            _isListView = false;
        }

        private async Task SubmitAsync()
        {
            try
            {
                _errorMessage = null;

                var isValidated = _editContextValidator.Validate();

                Console.WriteLine(isValidated);
                Console.WriteLine(string.Join(" ", _editContextValidator.EditContext.GetValidationMessages()));

                if (!isValidated)
                    return;

                _isSaving = true;

                var existingEntity = this.Ruleset.Weapons.SingleOrDefault(w => w.Id == _weapon.Id);

                var entity = existingEntity == null
                    ? await this.WeaponService.AddAsync(this.Ruleset.Id, _weapon)
                    : await this.WeaponService.UpdateAsync(this.Ruleset.Id, _weapon);

                if (existingEntity != null)
                    this.Ruleset.Weapons.Remove(existingEntity);

                this.Ruleset.Weapons.Add(entity);

                _isListView = true;

                await this.OnSaved.InvokeAsync(null);
            }
            catch(Exception exception)
            {
                Console.Write(exception.ToString());
                _errorMessage = Constants.SAVING_ERROR;
            }
            finally
            {
                _isSaving = false;
                base.StateHasChanged();
            }
        }

        private async Task PromptDeleteAsync(Guid id)
        {
            _errorMessage = null;
            await this.JSRuntime.InvokeVoidAsync(Scripts.Semantic.SHOW_DELETE_CONFIRMATION);
            _weaponId = id;
        }

        private async Task DeleteAsync()
        {
            var entity = this.Ruleset.Weapons.SingleOrDefault(e => e.Id == _weaponId);

            try
            {
                if (entity == null)
                    throw new InvalidOperationException();

                this.Ruleset.Weapons.Remove(entity);

                await this.WeaponService.DeleteAsync(entity.Id);
            }
            catch(Exception exception)
            {
                _errorMessage = Constants.DELETE_ERROR;

                // Undo client side removal.
                if (entity != null)
                    this.Ruleset.Weapons.Add(entity);

                Console.WriteLine(exception.ToString());
            }
        }

        private async Task CopyAsync(Guid id)
        {
            try
            {
                var entity = this.Ruleset.Weapons.Single(e => e.Id == id);
                var createdEntity = await this.WeaponService.AddAsync(this.Ruleset.Id, entity);

                this.Ruleset.Weapons.Add(createdEntity);
            }
            catch(Exception exception)
            {
                _errorMessage = Constants.COPY_ERROR;
                Console.WriteLine(exception.ToString());
            }
        }
    }
}
