using BlazorInputFile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Simucraft.Client.Common;
using Simucraft.Client.Components;
using Simucraft.Client.Models;
using Simucraft.Client.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Simucraft.Client.Pages
{
    [Authorize]
    public partial class MapCreateView : ComponentBase
    {
        private Map _map;

        private IEnumerable<Ruleset> _rulesets = new List<Ruleset>();

        private EditContextValidator _editContextValidator;

        private bool _isInitialized;
        private bool _isLoadingImage;
        private bool _isSaving;

        private string _errorMessage;
        private string _imageUrl;

        private IFileListEntry _fileListEntry;

        [Inject]
        private IRulesetService RulesetService { get; set; }

        [Inject]
        private IMapService MapService { get; set; }

        [Inject]
        private NavigationManager NavigationManager { get; set; }

        [Inject]
        private IJSRuntime JSRuntime { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            { 
                _map = Map.Empty;
                _editContextValidator = new EditContextValidator(_map);
            }
            catch (Exception exception)
            {
                _errorMessage = Constants.INITIALIZE_ERROR;
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
                return;

            try
            {
                _rulesets = await this.RulesetService.GetAllAsync();
                _map.RulesetId = _rulesets.FirstOrDefault()?.Id ?? Guid.Empty;

                _isInitialized = true;
                base.StateHasChanged();

                await this.JSRuntime.InvokeVoidAsync(Scripts.Semantic.DROPDOWN);
            }
            catch(Exception exception)
            {
                _errorMessage = Constants.INITIALIZE_ERROR;
            }
            finally
            {
                _isInitialized = true;
                base.StateHasChanged();
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

                if (file.Size > ByteSize.FromMegaBytes(5))
                    throw new InvalidOperationException("Image size canoot exceed 5 MB.");

                _isLoadingImage = true;

                _map.ImageName = file.Name.ToUpper();
                _fileListEntry = file;

                _editContextValidator.Validate();
            }
            catch (InvalidOperationException invalidOperationException)
            {
                _errorMessage = invalidOperationException.Message;
            }
            catch (Exception exception)
            {
                _errorMessage = Constants.IMAGE_ERROR;
            }
            finally
            {
                _isLoadingImage = false;
            }
        }


        private async Task SubmitAsync()
        {
            try
            {
                _errorMessage = null;

                var isValid = _editContextValidator.Validate();
                if (!isValid)
                    return;

                _isSaving = true;

                var map = await this.MapService.AddAsync(_map.RulesetId, _map);
                await this.MapService.SetImageAsync(map.Id, Path.GetFileName(_fileListEntry.Name), await _fileListEntry.ToByteArrayAsync());

                this.NavigationManager.NavigateTo($"/rulesets/{map.RulesetId}/mapview/{map.Id}");
            }
            catch(SubscriptionException subscriptionException)
            {
                _errorMessage = subscriptionException.Message;
            }
            catch(Exception exception)
            {
                _errorMessage = Constants.SAVING_ERROR;
            }
            finally
            {
                _isSaving = false;
            }
        }
    }
}
