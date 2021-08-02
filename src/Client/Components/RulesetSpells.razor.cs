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
    public partial class RulesetSpells : ComponentBase
    {
        private string _errorMessage;

        private bool _isListView;
        private bool _isSaving;

        private EditContextValidator _editContextValidator;

        private Spell _spell;
        private Guid _spellId;

        [Inject]
        private IJSRuntime  JSRuntime { get; set; }

        [Inject]
        private ISpellService SpellService { get; set; }

        [Parameter]
        public Ruleset Ruleset { get; set; }

        [Parameter]
        public EventCallback OnSaved { get; set; }

        protected override void OnInitialized()
        {
            _isListView = true;

            _spell = Spell.Empty;
            _editContextValidator = new EditContextValidator(_spell);

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
            _spell = id.HasValue
                ? this.Ruleset.Spells.Single(r => r.Id == id).Copy()
                : Spell.Empty;

            _editContextValidator = new EditContextValidator(_spell);
            _isListView = false;
        }

        private async Task SubmitAsync()
        {
            try
            {
                _errorMessage = null;

                var isValidated = _editContextValidator.Validate();
                if (!isValidated)
                    return;

                _isSaving = true;

                var existingEntity = this.Ruleset.Spells.SingleOrDefault(w => w.Id == _spell.Id);

                var entity = existingEntity == null
                    ? await this.SpellService.AddAsync(this.Ruleset.Id, _spell)
                    : await this.SpellService.UpdateAsync(this.Ruleset.Id, _spell);

                if (existingEntity != null)
                    this.Ruleset.Spells.Remove(existingEntity);

                this.Ruleset.Spells.Add(entity);

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
            }
        }

        private async Task PromptDeleteAsync(Guid id)
        {
            _errorMessage = null;
            await this.JSRuntime.InvokeVoidAsync(Scripts.Semantic.SHOW_DELETE_CONFIRMATION);
            _spellId = id;
        }

        private async Task DeleteAsync()
        {
            var entity = this.Ruleset.Spells.SingleOrDefault(e => e.Id == _spellId);

            try
            {
                if (entity == null)
                    throw new InvalidOperationException();

                this.Ruleset.Spells.Remove(entity);

                await this.SpellService.DeleteAsync(entity.Id);
            }
            catch(Exception exception)
            {
                _errorMessage = Constants.DELETE_ERROR;

                // Undo client side removal.
                if (entity != null)
                    this.Ruleset.Spells.Add(entity);

                Console.WriteLine(exception.ToString());
            }
        }

        private async Task CopyAsync(Guid id)
        {
            try
            {
                var entity = this.Ruleset.Spells.Single(e => e.Id == id);
                var createdEntity = await this.SpellService.AddAsync(this.Ruleset.Id, entity);

                this.Ruleset.Spells.Add(createdEntity);
            }
            catch(Exception exception)
            {
                _errorMessage = Constants.COPY_ERROR;
                Console.WriteLine(exception.ToString());
            }
        }
    }
}
