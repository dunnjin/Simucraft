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
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace Simucraft.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class GamesController : Controller
    {
        private readonly IGameService _gameService;

        public GamesController(IGameService gameService)
        {
            _gameService = gameService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var userId = base.User.GetId();
                var games = await _gameService.GetByUserIdAsync(userId);

                return base.Ok(games);
            }
            catch(Exception exception)
            {
                throw;
            }
        }

        [HttpGet("/api/[controller]/invited")]
        public async Task<IActionResult> GetInvited()
        {
            var userId = base.User.GetId();
            var games = await _gameService.GetByInvitedAsync(userId);

            return base.Ok(games);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            try
            {
                var userId = base.User.GetId();
                var game = await _gameService.GetByIdAsync(userId, id);
                if (game == null)
                    return base.NotFound();

                return base.Ok(game);
            }
            catch(Exception exception)
            {
                throw;
            }
        }

        [HttpPost("/api/rulesets/{rulesetId}/[controller]")]
        public async Task<IActionResult> Post(Guid rulesetId, GameRequest game)
        {
            try
            {
                var authorizedUser = new AuthorizedUserRequest
                {
                    DisplayName = base.User.GetDisplayName(),
                    UserId = base.User.GetId(),
                    EmailNormalized = base.User.GetEmail().ToUpper(),
                };
                var createdGame = await _gameService.AddAsync(rulesetId, authorizedUser, game);

                return base.Ok(createdGame);
            }
            catch(Exception exception)
            {
                throw;
            }
        }

        [HttpPut("/api/rulesets/{rulesetId}/[controller]/{id}")]
        public async Task<IActionResult> Put(Guid rulesetId, Guid id, GameRequest game)
        {
            try
            {
                var userId = base.User.GetId();
                var response = await _gameService.UpdateAsync(userId, rulesetId, id, game);

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
                await _gameService.DeleteAsync(userId, id);

                return base.Ok();
            }
            catch(Exception exception)
            {
                throw;
            }
        }

        [HttpPost("/api/rulesets/{rulesetId}/games/{id}/invite")]
        public async Task<IActionResult> Invite(Guid rulesetId, Guid id, AcceptInvite acceptInvite)
        {
            var authorizedUser = new AuthorizedUserRequest
            {
                DisplayName = base.User.GetDisplayName(),
                UserId = base.User.GetId(),
                EmailNormalized = base.User.GetEmail().ToUpper(),
            };

            await _gameService.AcceptInviteAsync(authorizedUser, rulesetId, id, acceptInvite.InviteId);

            return base.NoContent();
        }
    }
}
