using Simucraft.Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Simucraft.Client.Services
{
    public interface IGameService
    {
        Task<Game> GetByIdAsync(Guid id);

        Task<IEnumerable<Game>> GetAllAsync();
        Task<IEnumerable<Game>> GetInvitedAsync();

        Task<Game> AddAsync(Guid rulesetId, Game game);

        Task UpdateAsync(Guid rulesetId, Game game);

        Task DeleteAsync(Guid gameId);

        Task AcceptInviteAsync(Guid rulesetId, Guid gameId, string inviteId);
    }
}
