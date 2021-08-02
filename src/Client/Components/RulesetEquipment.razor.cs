using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Simucraft.Client.Common;
using Simucraft.Client.Core;
using Simucraft.Client.Models;
using Simucraft.Client.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Simucraft.Client.Components
{
    public partial class RulesetEquipment : ComponentBase
    {
        private string _errorMessage;

        private bool _isListView;
        private bool _isSaving;

        private EditContextValidator _editContextValidator;

        private Equipment _equipment;

        private Guid _equipmentId;

        private ICollection<int> _invalidExpressionSelf = new List<int>();
        private ICollection<int> _invalidExpressionTarget = new List<int>();

        [Inject]
        private IJSRuntime  JSRuntime { get; set; }

        [Inject]
        private IEquipmentService EquipmentService { get; set; }

        [Parameter]
        public Ruleset Ruleset { get; set; }

        [Parameter]
        public EventCallback OnSaved { get; set; }

        private EquipmentType EquipmentType
        {
            get => _equipment.EquipmentTypeParsed;
            set
            {
                _equipment.EquipmentTypeParsed = value;
                this.ClearEquipmentFromEquipmentType();
            }
        }

        protected override void OnInitialized()
        {
            _isListView = true;

            _equipment = Equipment.Empty;
            _editContextValidator = new EditContextValidator(_equipment);

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
            try
            {
                _equipment = id.HasValue
                    ? this.Ruleset.Equipment.Single(r => r.Id == id).Copy()
                    : Equipment.Empty;
                Console.WriteLine(_equipment.ToJson());
                if (!id.HasValue)
                {
                    if (this.Ruleset.Equipment.Count >= 100)
                        throw new InvalidOperationException("Cannot create more than 100 Equipment.");
                }

                _editContextValidator = new EditContextValidator(_equipment);
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

                if (_equipment.EquipmentTypeParsed == EquipmentType.Both || _equipment.EquipmentTypeParsed == EquipmentType.Passive)
                {
                    _invalidExpressionSelf = _equipment.PassiveExpressions
                        .Select((e, i) => !e.SelfExpression.IsNumberExpression() ? i : -1)
                        .Where(i => i != -1)
                        .ToList();
                    _invalidExpressionTarget = _equipment.PassiveExpressions
                        .Select((e, i) => !e.TargetExpression.IsNumberExpression() ? i : -1)
                        .Where(i => i != -1)
                        .ToList();
                }

                var isValidated = _editContextValidator.Validate();
                if (!isValidated || _invalidExpressionTarget.Any() || _invalidExpressionSelf.Any())
                    return;

                _isSaving = true;

                var existingEntity = this.Ruleset.Equipment.SingleOrDefault(w => w.Id == _equipment.Id);

                var entity = existingEntity == null
                    ? await this.EquipmentService.AddAsync(this.Ruleset.Id, _equipment)
                    : await this.EquipmentService.UpdateAsync(this.Ruleset.Id, _equipment);

                if (existingEntity != null)
                    this.Ruleset.Equipment.Remove(existingEntity);

                this.Ruleset.Equipment.Add(entity);

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
            _equipmentId = id;
        }

        private async Task DeleteAsync()
        {
            var entity = this.Ruleset.Weapons.SingleOrDefault(e => e.Id == _equipmentId);

            try
            {
                if (entity == null)
                    throw new InvalidOperationException();

                this.Ruleset.Weapons.Remove(entity);

                await this.EquipmentService.DeleteAsync(entity.Id);
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
                var entity = this.Ruleset.Equipment.Single(e => e.Id == id);
                var createdEntity = await this.EquipmentService.AddAsync(this.Ruleset.Id, entity);

                this.Ruleset.Equipment.Add(createdEntity);
            }
            catch(Exception exception)
            {
                _errorMessage = Constants.COPY_ERROR;
                Console.WriteLine(exception.ToString());
            }
        }

        private void AddExpression()
        {
            if (_equipment.PassiveExpressions.Count >= 5)
                return;

            _equipment.PassiveExpressions.Add(EquipmentExpression.Empty);
        }

        private void RemoveExpression(int index)
        {
            var expression = _equipment.PassiveExpressions.ElementAtOrDefault(index);
            if (expression == null)
                return;

            _equipment.PassiveExpressions.Remove(expression);
        }

        private void ClearEquipmentFromEquipmentType()
        {
            Console.WriteLine($"before " + _equipment.ToJson());

            _equipment.DamageTraits = null;
            _equipment.HitChanceOperatorExpression = ">";
            _equipment.HitChanceSelfExpression = null;
            _equipment.HitChanceTargetExpression = null;
            _equipment.ImmunityTraits = null;
            _equipment.PassiveExpressions.Clear();
            _equipment.ResistanceTraits = null;
            _equipment.Range = 1;
            _equipment.DamageExpression = null;
            _equipment.DamageOperatorExpression = "+";
            _equipment.CriticalDamageExpression = null;
            _equipment.CriticalHitChanceOperatorExpression = ">";
            _equipment.CriticalHitChanceSelfExpression = null;
            _equipment.CriticalHitChanceTargetExpression = null;

            _invalidExpressionSelf.Clear();
            _invalidExpressionTarget.Clear();

            _editContextValidator.NotifyField(nameof(_equipment.HitChanceSelfExpression));
            _editContextValidator.NotifyField(nameof(_equipment.HitChanceTargetExpression));
            _editContextValidator.NotifyField(nameof(_equipment.CriticalHitChanceSelfExpression));
            _editContextValidator.NotifyField(nameof(_equipment.CriticalDamageExpression));
            _editContextValidator.NotifyField(nameof(_equipment.CriticalHitChanceSelfExpression));
            _editContextValidator.NotifyField(nameof(_equipment.CriticalHitChanceTargetExpression));
            _editContextValidator.NotifyField(nameof(_equipment.DamageExpression));

            if (_equipment.EquipmentTypeParsed == EquipmentType.Passive || _equipment.EquipmentTypeParsed == EquipmentType.Both)
                this.AddExpression();

            Console.WriteLine($"after " + _equipment.ToJson());
        }
    }
}
