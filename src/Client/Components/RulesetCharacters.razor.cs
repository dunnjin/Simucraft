using BlazorInputFile;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Simucraft.Client.Common;
using Simucraft.Client.Core;
using Simucraft.Client.Models;
using Simucraft.Client.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Simucraft.Client.Components
{
    public partial class RulesetCharacters : ComponentBase
    {
        private EditContextValidator _editContextValidator;
        private Character _character;
        private string _errorMessage;
        private bool _isListView;
        private bool _isSaving;
        private bool _isLoadingImage;
        private Guid _selectedCharacterId;
        private IFileListEntry _imageFile;

        private IList<string> _invalidStatNames = new List<string>();
        private IList<string> _invalidSkillNames = new List<string>();
        private IList<string> _invalidSkillExpressions = new List<string>();

        [Inject]
        private IJSRuntime JSRuntime { get; set; }

        [Inject]
        private ICharacterService CharacterService { get; set; }

        public string WeaponIds
        {
            get => string.Join(",", _character.WeaponIds);
            set
            {
                _character.WeaponIds = value
                    ?.Split(",", StringSplitOptions.RemoveEmptyEntries)
                     .Select(e => Guid.Parse(e))
                     .ToList() ?? new List<Guid>();
            }
        }

        public string EquipmentIds
        {
            get => string.Join(",", _character.EquipmentIds);
            set
            {
                _character.EquipmentIds = value
                    ?.Split(",", StringSplitOptions.RemoveEmptyEntries)
                     .Select(e => Guid.Parse(e))
                     .ToList() ?? new List<Guid>();
            }
        }
        public string SpellIds
        {
            get => string.Join(",", _character.SpellIds);
            set
            {
                _character.SpellIds = value
                    ?.Split(",", StringSplitOptions.RemoveEmptyEntries)
                     .Select(e => Guid.Parse(e))
                     .ToList() ?? new List<Guid>();
            }
        }

        [Parameter]
        public Ruleset Ruleset { get; set; }

        protected override void OnInitialized()
        {
            _isListView = true;
            base.OnInitialized();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await this.JSRuntime.InvokeVoidAsync(Scripts.Semantic.DROPDOWN);
        }

        private void NavigateToCreate(Guid? id)
        {
            _character = id.HasValue
                ? this.Ruleset.Characters.Single(c => c.Id == id).Copy()
                : Character.Empty;

            var newKeywords = this.Ruleset
                .GenerateKeywords()
                .Select(s => s.ToLower())
                .Except(_character.Stats.Select(s => s.Name.ToLower()))
                .ToList();

            if (newKeywords.Any())
            {
                _character.Stats.AddRange(newKeywords
                    .Select(k =>
                        new Stat
                        {
                            Name = k,
                        }));
            }

            _editContextValidator = new EditContextValidator(_character);
            _isListView = false;
        }

        private async Task PromptDeleteAsync(Guid id)
        {
            _errorMessage = null;
            await this.JSRuntime.InvokeVoidAsync(Scripts.Semantic.SHOW_DELETE_CONFIRMATION);
            _selectedCharacterId = id;
        }

        private async Task DeleteAsync()
        {
            var deleteCharacter = this.Ruleset.Characters.SingleOrDefault(c => c.Id == _selectedCharacterId);

            try
            {

                if (deleteCharacter == null)
                    throw new InvalidOperationException();

                this.Ruleset.Characters.Remove(deleteCharacter);

                await this.CharacterService.DeleteAsync(deleteCharacter.Id);
            }
            catch (Exception exception)
            {
                if (deleteCharacter != null)
                    this.Ruleset.Characters.Add(deleteCharacter);

                _errorMessage = Constants.DELETE_ERROR;
            }
        }

        private async Task SubmitAsync()
        {

            try
            {
                _errorMessage = null;

                _invalidStatNames = _character.Stats
                    .Select(s => s.Name)
                    .Where(s => string.IsNullOrEmpty(s) ||
                               !s.RegexMatches(CalculationExtensions.EXPRESSION_REGEX).Any())
                    .ToList();

                _invalidSkillNames = _character.Skills
                    .Select(s => s.Name)
                    .Where(s => string.IsNullOrEmpty(s))
                    .ToList();

                _invalidSkillExpressions = _character.Skills
                    .Select(s => s.Expression)
                    .Where(s => string.IsNullOrEmpty(s) ||
                               !s.IsNumberExpression())
                    .ToList();

                var isValidated = _editContextValidator.Validate();
                if (!isValidated || _invalidStatNames.Any() || _invalidSkillExpressions.Any() || _invalidSkillNames.Any())
                    return;

                _isSaving = true;

                // Clean up stats.
                _character.Stats = _character.Stats
                    .DistinctBy(s => s.Name)
                    .Where(s => !Constants.RESERVED_KEYWORDS.Contains(s.Name.ToLower()))
                    .OrderBy(s => s.Name)
                    .ToList();
                _character.Skills = _character.Skills
                    .OrderBy(s => s.Category)
                    .ThenBy(s => s.Name)
                    .ToList();

                var existingCharacter = this.Ruleset.Characters.SingleOrDefault(c => c.Id == _character.Id);
                var entity = existingCharacter == null
                    ? await this.CharacterService.AddAsync(this.Ruleset.Id, _character)
                    : await this.CharacterService.UpdateAsync(this.Ruleset.Id, _character);

                // If a new image has been set, save if off.
                if(_imageFile != null)
                {
                    var imageUrl = await this.CharacterService.SetImageAsync(entity.Id, entity.ImageName, await _imageFile.ToByteArrayAsync());

                    /**
                     * Hack:
                     * If the entity was updated we need to trick the browser cache to refresh the image. 
                     * This should only be added in memory and not persisted.
                     */
                    entity.ImageUrl = existingCharacter == null ? imageUrl : $"{imageUrl}?t={DateTime.Now}";
                    _imageFile = null;
                }

                // Update UI.
                if (existingCharacter != null)
                    this.Ruleset.Characters.Remove(existingCharacter);

                this.Ruleset.Characters.Add(entity);

                _isListView = true;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.ToString());
                _errorMessage = Constants.SAVING_ERROR;
            }
            finally
            {
                _isSaving = false;
            }
        }

        private async Task FileUploadedAsync(IFileListEntry[] fileListEntries)
        {
            try
            {
                _errorMessage = null;
                var file = fileListEntries.FirstOrDefault();
                if (file == null)
                    return;

                if (file.Size > ByteSize.FromMegaBytes(1))
                    throw new InvalidOperationException("Image cannot exceed 1 MB.");

                _isLoadingImage = true;
                //var base64 = await file.ToBase64Async();

                _imageFile = file;
                _character.ImageName = Path.GetFileName(file.Name);
                //_character.ImageMediaType = file.Type;
            }
            catch(InvalidOperationException invalidOperationException)
            {
                _errorMessage = invalidOperationException.Message;
            }
            catch (Exception exception)
            {
                _errorMessage = "An error occured, make sure it's a valid image.";
            }
            finally
            {
                _isLoadingImage = false;
            }
        }

        private async Task CopyAsync(Guid id)
        {
            try
            {
                var entity = this.Ruleset.Characters.Single(e => e.Id == id);
                var createdEntity = await this.CharacterService.AddAsync(this.Ruleset.Id, entity);

                if (!string.IsNullOrEmpty(entity.ImageUrl))
                {
                    var imageUrl = await this.CharacterService.CopyImageAsync(entity.Id, createdEntity.Id);
                    createdEntity.ImageUrl = imageUrl;
                }

                this.Ruleset.Characters.Add(createdEntity);
            }
            catch (Exception exception)
            {
                _errorMessage = Constants.COPY_ERROR;
                Console.WriteLine(exception.ToString());
            }
        }

        private void AddStat()
        {
            _character.Stats.Insert(0, Stat.Empty);
        }

        private void RemoveStat(string statName)
        {
            var characterStat = _character.Stats.FirstOrDefault(s => s.Name?.ToLower() == statName?.ToLower());
            if (characterStat != null)
                _character.Stats.Remove(characterStat);
        }

        private void AddSkill()
        {
            _character.Skills.Insert(0, CharacterSkill.Empty);
        }

        private void RemoveSkill(Guid skillId)
        {
            var characterSkill = _character.Skills.FirstOrDefault(s => s.Id == skillId);
            if (characterSkill != null)
                _character.Skills.Remove(characterSkill);
        }
    }
}
