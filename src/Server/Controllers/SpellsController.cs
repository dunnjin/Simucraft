using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Simucraft.Server.Common;
using Simucraft.Server.Core;
using Simucraft.Server.Services;
using System;
using System.Threading.Tasks;

namespace Simucraft.Server.Controllers
{
    [Authorize]
    [ApiController]
    public class SpellsController : Controller
    {
        private readonly ISpellService _skillService;

        public SpellsController(ISpellService skillService)
        {
            _skillService = skillService;
        }

        [HttpGet("/api/rulesets/{rulesetId}/[controller]")]
        public async Task<IActionResult> GetByRuleset(Guid rulesetId)
        {
            try
            {
                var userId = base.User.GetId();
                var entities = await _skillService.GetByRulesetIdAsync(userId, rulesetId);

                return base.Ok(entities);
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
                var entity = await _skillService.GetByIdAsync(userId, id);

                if (entity == null)
                    return base.NotFound();

                return base.Ok(entity);
            }
            catch(Exception execption)
            {
                throw;
            }
        }

        [HttpPost("/api/rulesets/{rulesetId}/[controller]")]
        public async Task<IActionResult> Post(Guid rulesetId, SpellRequest request)
        {
            try
            {
                var userId = base.User.GetId();
                var entity = await _skillService.AddAsync(userId, rulesetId, request);

                return base.Ok(entity);
            }
            catch(Exception exception)
            {
                throw;
            }
        }

        [HttpPut("/api/rulesets/{rulesetId}/[controller]/{id}")]
        public async Task<IActionResult> Put(Guid rulesetId, Guid id, SpellRequest request)
        {
            try
            {
                var userId = base.User.GetId();
                var response = await _skillService.UpdateAsync(userId, rulesetId, id, request);

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
                await _skillService.DeleteAsync(userId, id);

                return base.Ok();
            }
            catch(Exception exception)
            {
                throw;
            }
        }
    }
}
