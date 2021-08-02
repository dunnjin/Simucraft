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
    public class EquipmentService : IEquipmentService
    {
        private const string API_URI = "/api/equipment";

        private HttpClient _httpClient;

        public EquipmentService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Equipment> GetByIdAsync(Guid id)
        {
            try
            {
                var entity = await _httpClient.GetFromJsonAsync<Equipment>($"{API_URI}/{id}");
                return entity;
            }
            catch(HttpRequestException httpRequestException)
            {
                if (httpRequestException.Message.Contains("404"))
                    throw new InvalidOperationException("Equipment not found.");

                throw;
            }
        }

        public async Task<IEnumerable<Equipment>> GetAllByRulesetIdAsync(Guid rulesetId)
        {
            var entities = await _httpClient.GetFromJsonAsync<List<Equipment>>($"/api/rulesets/{rulesetId}/equipment");
            return entities;
        }

        public async Task<Equipment> AddAsync(Guid rulesetId, Equipment equipment)
        {
            var response = await Task.Run(async () => await _httpClient.PostAsJsonAsync($"/api/rulesets/{rulesetId}/equipment", equipment));
            response.EnsureSuccessStatusCode();

            var entity = await response.Content.ReadFromJsonAsync<Equipment>();
            return entity;
        }

        public async Task<Equipment> UpdateAsync(Guid rulesetId, Equipment equipment)
        {
            var response = await Task.Run(async () => await _httpClient.PutAsJsonAsync($"/api/rulesets/{rulesetId}/equipment/{equipment.Id}", equipment));
            response.EnsureSuccessStatusCode();

            var entity = await response.Content.ReadFromJsonAsync<Equipment>();
            return entity;
        }

        public async Task DeleteAsync(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"{API_URI}/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}
