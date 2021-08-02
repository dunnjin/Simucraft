using Simucraft.Client.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Simucraft.Client.Services
{
    public class SpellService  : ISpellService
    {
        private const string API_URI = "/api/spells";

        private HttpClient _httpClient;

        public SpellService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Spell> GetByIdAsync(Guid id)
        {
            try
            {
                var entity = await _httpClient.GetFromJsonAsync<Spell>($"{API_URI}/{id}");
                return entity;
            }
            catch(HttpRequestException httpRequestException)
            {
                if (httpRequestException.Message.Contains("404"))
                    throw new InvalidOperationException("Spell not found.");

                throw;
            }
        }

        public async Task<IEnumerable<Spell>> GetAllByRulesetIdAsync(Guid rulesetId)
        {
            var entities = await _httpClient.GetFromJsonAsync<List<Spell>>($"/api/rulesets/{rulesetId}/spells");
            return entities;
        }

        public async Task<Spell> AddAsync(Guid rulesetId, Spell skill)
        {
            var response = await Task.Run(async () => await _httpClient.PostAsJsonAsync($"/api/rulesets/{rulesetId}/spells", skill));
            response.EnsureSuccessStatusCode();

            var entity = await response.Content.ReadFromJsonAsync<Spell>();
            return entity;
        }

        public async Task<Spell> UpdateAsync(Guid rulesetId, Spell weapon)
        {
            var response = await Task.Run(async () => await _httpClient.PutAsJsonAsync($"/api/rulesets/{rulesetId}/spells/{weapon.Id}", weapon));
            response.EnsureSuccessStatusCode();

            var entity = await response.Content.ReadFromJsonAsync<Spell>();
            return entity;
        }

        public async Task DeleteAsync(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"{API_URI}/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}
