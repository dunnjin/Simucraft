using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Simucraft.Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Simucraft.Client.Services
{
    public class WeaponService : IWeaponService
    {
        private const string API_URI = "/api/weapons";

        private HttpClient _httpClient;

        public WeaponService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Weapon> GetByIdAsync(Guid id)
        {
            try
            {
                var entity = await _httpClient.GetFromJsonAsync<Weapon>($"{API_URI}/{id}");
                return entity;
            }
            catch(HttpRequestException httpRequestException)
            {
                if (httpRequestException.Message.Contains("404"))
                    throw new InvalidOperationException("Weapon not found.");

                throw;
            }
        }

        public async Task<IEnumerable<Weapon>> GetAllByRulesetIdAsync(Guid rulesetId)
        {
            var entities = await _httpClient.GetFromJsonAsync<List<Weapon>>($"/api/rulesets/{rulesetId}/weapons");
            return entities;
        }

        public async Task<Weapon> AddAsync(Guid rulesetId, Weapon weapon)
        {
            var response = await Task.Run(async () => await _httpClient.PostAsJsonAsync($"/api/rulesets/{rulesetId}/weapons", weapon));
            response.EnsureSuccessStatusCode();

            var entity = await response.Content.ReadFromJsonAsync<Weapon>();
            return entity;
        }

        public async Task<Weapon> UpdateAsync(Guid rulesetId, Weapon weapon)
        {
            var response = await Task.Run(async () => await _httpClient.PutAsJsonAsync($"/api/rulesets/{rulesetId}/weapons/{weapon.Id}", weapon));
            response.EnsureSuccessStatusCode();

            var entity = await response.Content.ReadFromJsonAsync<Weapon>();
            return entity;
        }

        public async Task DeleteAsync(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"{API_URI}/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}
