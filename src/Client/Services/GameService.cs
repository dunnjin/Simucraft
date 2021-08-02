using Simucraft.Client.Common;
using Simucraft.Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Simucraft.Client.Services
{
    public class GameService : IGameService
    {
        private const string GAME_URI = "/api/games";

        private HttpClient _httpClient;

        public GameService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Game> GetByIdAsync(Guid id)
        {
            try
            {
                var game = await _httpClient.GetFromJsonAsync<Game>($"{GAME_URI}/{id}");
                return game;
            }
            catch(HttpRequestException httpRequestException)
            {
                if (httpRequestException.Message.Contains("404"))
                    throw new InvalidOperationException("Game not found.");

                throw;
            }
        }

        public async Task<IEnumerable<Game>> GetAllAsync()
        {
            var games = await _httpClient.GetFromJsonAsync<List<Game>>(GAME_URI);
            return games;
        }

        public async Task<IEnumerable<Game>> GetInvitedAsync()
        {
            var games = await _httpClient.GetFromJsonAsync<List<Game>>($"{GAME_URI}/invited");
            return games;
        }

        public async Task<Game> AddAsync(Guid rulesetId, Game game)
        {
            var response = await Task.Run(async () => await _httpClient.PostAsJsonAsync($"/api/rulesets/{rulesetId}/games", game));
            response.EnsureSuccessStatusCode();

            var createdGame = await response.Content.ReadFromJsonAsync<Game>();
            return createdGame;
        }

        public async Task UpdateAsync(Guid rulesetId, Game game)
        {
            // Serialization appears to not be ran async.
            var response = await Task.Run(async () => await _httpClient.PutAsJsonAsync($"/api/rulesets/{rulesetId}/games/{game.Id}", game));
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteAsync(Guid gameId)
        {
            var response = await _httpClient.DeleteAsync($"{GAME_URI}/{gameId}");
            response.EnsureSuccessStatusCode();
        }

        public async Task AcceptInviteAsync(Guid rulesetId, Guid gameId, string inviteId)
        {
            var response = await _httpClient.PostAsJsonAsync($"/api/rulesets/{rulesetId}/games/{gameId}/invite", new { inviteId });
            if (response.StatusCode == HttpStatusCode.BadRequest)
                throw new InvalidOperationException("Bad Request.");

            response.EnsureSuccessStatusCode();
        }
    }
}
