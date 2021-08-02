using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Simucraft.Server.Common;
using Simucraft.Server.Core;
using Simucraft.Server.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Simucraft.Server.Controllers
{
    [Authorize]
    [ApiController]
    public class CharactersController : Controller
    {
        private readonly ICharacterService _characterService;

        public CharactersController(ICharacterService characterService)
        {
            _characterService = characterService;
        }

        [HttpGet("/api/rulesets/{rulesetId}/[controller]")]
        public async Task<IActionResult> GetByRuleset(Guid rulesetId)
        {
            try
            {
                var userId = base.User.GetId();
                var characters = await _characterService.GetByRulesetIdAsync(userId, rulesetId);

                return base.Ok(characters);
            }
            catch(Exception exception)
            {
                throw;
            }
        }

        [HttpGet("/api/[controller]/{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            try
            {
                var userId = base.User.GetId();
                var character = await _characterService.GetByIdAsync(userId, id);

                if (character == null)
                    return base.NotFound();

                return base.Ok(character);
            }
            catch(Exception execption)
            {
                throw;
            }
        }

        [HttpPost("/api/rulesets/{rulesetId}/[controller]")]
        public async Task<IActionResult> Post(Guid rulesetId, CharacterRequest characterRequest)
        {
            try
            {
                var userId = base.User.GetId();
                var createdCharacter = await _characterService.AddAsync(userId, rulesetId, characterRequest);

                return base.Ok(createdCharacter);
            }
            catch(Exception exception)
            {
                throw;
            }
        }

        [HttpPut("/api/rulesets/{rulesetId}/[controller]/{id}")]
        public async Task<IActionResult> Put(Guid rulesetId, Guid id, CharacterRequest characterRequest)
        {
            try
            {
                var userId = base.User.GetId();
                var response = await _characterService.UpdateAsync(userId, rulesetId, id, characterRequest);

                return base.Ok(response);
            }
            catch(Exception exception)
            {
                throw;
            }
        }

        [HttpDelete("/api/[controller]/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var userId = base.User.GetId();
                await _characterService.DeleteAsync(userId, id);

                return base.Ok();
            }
            catch(Exception exception)
            {
                throw;
            }
        }

        [HttpPost("/api/[controller]/{id}/image")]
        public async Task<IActionResult> Image(Guid id, [FromForm(Name = "file")] IFormFile formFile)
        {
            try
            {
                var userId = base.User.GetId();
                var url = await _characterService.SetImageAsync(userId, id, formFile);

                return base.Ok(url);
            }
            catch(Exception exception)
            {
                throw;
            }
        }

        [HttpPost("/api/[controller]/{id}/copyimage")]
        public async Task<IActionResult> CopyImage(Guid id, CopyImageRequest copyImageRequest)
        {
            try
            {
                var userId = base.User.GetId();
                var url = await _characterService.CopyImageAsync(userId, copyImageRequest.SourceCharacterId, id);

                return base.Ok(url);
            }
            catch(Exception exception)
            {
                throw;
            }
        }
    }
}
