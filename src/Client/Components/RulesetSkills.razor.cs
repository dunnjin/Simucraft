using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Simucraft.Client.Common;
using Simucraft.Client.Core;
using Simucraft.Client.Models;
using Simucraft.Client.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Simucraft.Client.Components
{
    public partial class RulesetSkills : ComponentBase
    {
        private string _errorMessage;

        private bool _isListView;
        private bool _isSaving;

        private EditContextValidator _editContextValidator;

        private Skill _skill;
        private Guid _skillId;

        private ICollection<int> _invalidExpressionTarget = new List<int>();
        private ICollection<int> _invalidExpressionSelf = new List<int>();

        [Inject]
        private IJSRuntime JSRuntime { get; set; }

        [Inject]
        private ISkillService SkillService { get; set; }

        [Parameter]
        public Ruleset Ruleset { get; set; }

        [Parameter]
        public EventCallback OnSaved { get; set; }

        private SkillType SkillType
        {
            get => _skill.SkillType;
            set
            {
                _skill.SkillType = value;
                _skill.Expressions = new List<SkillExpression> { SkillExpression.Empty };
                _skill.ImmunityTraits = null;
                _skill.ResistanceTraits = null;
            }
        }

        protected override void OnInitialized()
        {
            _isListView = true;

            _skill = Skill.Empty;
            _editContextValidator = new EditContextValidator(_skill);

            base.OnInitialized();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            try
            {
                await this.JSRuntime.InvokeVoidAsync(Scripts.Semantic.DROPDOWN);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.ToString());
            }
        }

        private void NavigateToCreate(Guid? id)
        {
            try
            {
                _skill = id.HasValue
                    ? this.Ruleset.Skills.Single(r => r.Id == id).Copy()
                    : Skill.Empty;

                if (!id.HasValue)
                {
                    if (this.Ruleset.Skills.Count >= 100)
                        throw new InvalidOperationException("Cannot create more than 100 Skills.");

                    _skill.Expressions.Add(SkillExpression.Empty);
                }

                _editContextValidator = new EditContextValidator(_skill);
                _isListView = false;
            }
            catch(InvalidOperationException invalidOperationException)
            {
                _errorMessage = invalidOperationException.Message;
            }
            catch(Exception exception)
            {

            }
        }

        private async Task SubmitAsync()
        {
            try
            {
                _errorMessage = null;
                _invalidExpressionSelf.Clear();
                _invalidExpressionTarget.Clear();

                //_invalidExpressions = _skill.Expressions
                //    .Select((e, i) => !e.Expression.IsNumberExpression() ? i : -1)
                //    .Where(i => i != -1)
                //    .ToList();
                //_invalidNames = _skill.Expressions
                //    .Select((e, i) => string.IsNullOrEmpty(e.Name) ? i : -1)
                //    .Where(i => i != -1)
                //    .ToList();

                var isValidated = _editContextValidator.Validate();
                if (!isValidated || _invalidExpressionTarget.Any() || _invalidExpressionSelf.Any())
                    return;

                _isSaving = true;

                var existingEntity = this.Ruleset.Skills.SingleOrDefault(w => w.Id == _skill.Id);

                var entity = existingEntity == null
                    ? await this.SkillService.AddAsync(this.Ruleset.Id, _skill)
                    : await this.SkillService.UpdateAsync(this.Ruleset.Id, _skill);

                if (existingEntity != null)
                    this.Ruleset.Skills.Remove(existingEntity);

                this.Ruleset.Skills.Add(entity);

                _isListView = true;

                await this.OnSaved.InvokeAsync(null);
            }
            catch (Exception exception)
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
            _skillId = id;
        }

        private async Task DeleteAsync()
        {
            var entity = this.Ruleset.Skills.SingleOrDefault(e => e.Id == _skillId);

            try
            {
                if (entity == null)
                    throw new InvalidOperationException();

                this.Ruleset.Skills.Remove(entity);

                await this.SkillService.DeleteAsync(entity.Id);
            }
            catch (Exception exception)
            {
                _errorMessage = Constants.DELETE_ERROR;

                // Undo client side removal.
                if (entity != null)
                    this.Ruleset.Skills.Add(entity);

                Console.WriteLine(exception.ToString());
            }
        }

        private async Task CopyAsync(Guid id)
        {
            try
            {
                var entity = this.Ruleset.Skills.Single(e => e.Id == id);
                var createdEntity = await this.SkillService.AddAsync(this.Ruleset.Id, entity);

                this.Ruleset.Skills.Add(createdEntity);
            }
            catch (Exception exception)
            {
                _errorMessage = Constants.COPY_ERROR;
                Console.WriteLine(exception.ToString());
            }
        }

        private void AddExpression()
        {
            try
            {
                if (_skill.Expressions.Count > 5)
                    throw new InvalidOperationException("Only 5 Rolls may be added to Skill.");

                _skill.Expressions.Add(SkillExpression.Empty);
            }
            catch(InvalidOperationException invalidOperationException)
            {
                _errorMessage = invalidOperationException.Message;
            }
        }

        private void RemoveExpression(int index)
        {
            var element = _skill.Expressions.ElementAtOrDefault(index);
            if (element != null)
                _skill.Expressions.Remove(element);
        }
    }
}
