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
    public class EquipmentController : Controller
    {
        private readonly IEquipmentService _equipmentService;

        public EquipmentController(IEquipmentService equipmentService)
        {
            _equipmentService = equipmentService;
        }

        [HttpGet("/api/rulesets/{rulesetId}/[controller]")]
        public async Task<IActionResult> GetByRuleset(Guid rulesetId)
        {
            try
            {
                var userId = base.User.GetId();
                var entities = await _equipmentService.GetByRulesetIdAsync(userId, rulesetId);

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
                var entity = await _equipmentService.GetByIdAsync(userId, id);

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
        public async Task<IActionResult> Post(Guid rulesetId, EquipmentRequest request)
        {
            try
            {
                var userId = base.User.GetId();
                var entity = await _equipmentService.AddAsync(userId, rulesetId, request);

                return base.Ok(entity);
            }
            catch(Exception exception)
            {
                throw;
            }
        }

        [HttpPut("/api/rulesets/{rulesetId}/[controller]/{id}")]
        public async Task<IActionResult> Put(Guid rulesetId, Guid id, EquipmentRequest request)
        {
            try
            {
                var userId = base.User.GetId();
                var response = await _equipmentService.UpdateAsync(userId, rulesetId, id, request);

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
                await _equipmentService.DeleteAsync(userId, id);

                return base.Ok();
            }
            catch(Exception exception)
            {
                throw;
            }
        }
    }
}
