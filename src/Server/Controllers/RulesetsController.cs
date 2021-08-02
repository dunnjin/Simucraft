using Microsoft.AspNetCore.Authorization;
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
    //[Route("api/[controller]")]
    public class RulesetsController : Controller
    {
        private readonly IRulesetService _rulesetService;

        public RulesetsController(IRulesetService rulesetService)
        {
            _rulesetService = rulesetService;
        }

        [HttpPost("/api/[controller]")]
        public async Task<IActionResult> Post(RulesetRequest rulesetRequest)
        {
            try
            {
                var userId = base.User.GetId();
                var ruleset = await _rulesetService.AddAsync(userId, rulesetRequest);

                return base.Ok(ruleset);
            }
            catch(Exception exception)
            {
                throw;
            }
        }

        [HttpPut("/api/[controller]/{id}")]
        public async Task<IActionResult> Put(Guid id, RulesetRequest rulesetRequest)
        {
            try
            {
                var userId = base.User.GetId();
                var ruleset = await _rulesetService.UpdateAsync(userId, id, rulesetRequest);

                return base.Ok(ruleset);
            }
            catch(Exception exception)
            {
                throw;
            }
        }

        [HttpGet("/api/[controller]")]
        public async Task<IActionResult> Get()
        {
            try
            {
                var userId = base.User.GetId();
                var rulesets = await _rulesetService.GetByUserIdAsync(userId);

                return base.Ok(rulesets);
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
                var ruleset = await _rulesetService.GetByIdAsync(userId, id);

                if (ruleset == null)
                    return base.NotFound();

                return base.Ok(ruleset);
            }
            catch(Exception exception)
            {
                throw;
            }
        }

        [HttpPost("/api/[controller]/import")]
        public async Task<IActionResult> Import(RulesetImportRequest request)
        {
            try
            {
                var userId = base.User.GetId();
                var ruleset = await _rulesetService.ImportAsync(userId, request.TemplateId);

                return base.Ok(ruleset);
            }
            catch(Exception exception)
            {
                throw;
            }
        }

        // TODO: Need to finalize maps/games until I can allow deleting the ruleset, since this deletes everything associated with it.
        //[HttpDelete("/api/[controller]/{id}")]
        //public async Task<IActionResult> Delete(Guid id)
        //{
        //    var userId = base.User.GetId();
        //    await _rulesetService.DeleteAsync(userId, id);

        //    return base.NoContent();
        //}
    }
}
