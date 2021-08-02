using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Simucraft.Server.Common;
using Simucraft.Server.Services;
using Simucraft.Server.Models;
using Simucraft.Server.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Simucraft.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class MapsController : Controller
    {
        private readonly IMapService _mapService;

        public MapsController(IMapService mapService)
        {
            _mapService = mapService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var userId = base.User.GetId();
                var maps = await _mapService.GetByUserIdAsync(userId);

                return base.Ok(maps);
            }
            catch(Exception exception)
            {
                throw;
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            try
            {
                var userId = base.User.GetId();
                var map = await _mapService.GetByIdAsync(userId, id);
                if (map == null)
                    return base.NotFound();

                return base.Ok(map);
            }
            catch(Exception exception)
            {
                throw;
            }
        }

        [HttpGet("/api/rulesets/{rulesetId}/[controller]")]
        public async Task<IActionResult> GetByRuleset(Guid rulesetId)
        {
            try
            {
                var userId = base.User.GetId();
                var maps = await _mapService.GetByRulesetIdAsync(userId, rulesetId);

                return base.Ok(maps);
            }
            catch(Exception exception)
            {
                throw;
            }
        }

        [HttpPost("/api/rulesets/{rulesetId}/[controller]")]
        public async Task<IActionResult> Post(Guid rulesetId, MapRequest map)
        {
            try
            {
                var userId = base.User.GetId();
                var createdMap = await _mapService.AddAsync(userId, rulesetId, map);

                return base.Ok(createdMap);
            }
            catch(Exception exception)
            {
                throw;
            }
        }

        [HttpPut("/api/rulesets/{rulesetId}/[controller]/{id}")]

        public async Task<IActionResult> Put(Guid rulesetId, Guid id, MapRequest map)
        {
            try
            {
                var userId = base.User.GetId();
                var response = await _mapService.UpdateAsync(userId, rulesetId, id, map);

                return base.Ok(response);
            }
            catch(Exception exception)
            {
                throw;
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var userId = base.User.GetId();
                await _mapService.DeleteAsync(userId, id);

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
                await _mapService.SetImageAsync(userId, id, formFile);

                return base.Ok();
            }
            catch (Exception exception)
            {
                throw;
            }
        }
    }
}
