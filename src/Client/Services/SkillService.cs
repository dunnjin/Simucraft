using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Simucraft.Client.Common;
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
    public class SkillService : ISkillService
    {
        private const string API_URI = "/api/skills";

        private HttpClient _httpClient;

        public SkillService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Skill> GetByIdAsync(Guid id)
        {
            try
            {
                var entity = await _httpClient.GetFromJsonAsync<Skill>($"{API_URI}/{id}");
                return entity;
            }
            catch(HttpRequestException httpRequestException)
            {
                if (httpRequestException.Message.Contains("404"))
                    throw new InvalidOperationException("Skill not found.");

                throw;
            }
        }

        public async Task<IEnumerable<Skill>> GetAllByRulesetIdAsync(Guid rulesetId)
        {
            var entities = await _httpClient.GetFromJsonAsync<List<Skill>>($"/api/rulesets/{rulesetId}/skills");
            return entities;
        }

        public async Task<Skill> AddAsync(Guid rulesetId, Skill skill)
        {
            var response = await Task.Run(async () => await _httpClient.PostAsJsonAsync($"/api/rulesets/{rulesetId}/skills", skill));
            if(!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                    throw new MaxEntityException((await response.Content.ReadFromJsonAsync<ApiException>()).Message);

                response.EnsureSuccessStatusCode();
            }

            var entity = await response.Content.ReadFromJsonAsync<Skill>();
            return entity;
        }

        public async Task<Skill> UpdateAsync(Guid rulesetId, Skill skill)
        {
            var response = await Task.Run(async () => await _httpClient.PutAsJsonAsync($"/api/rulesets/{rulesetId}/skills/{skill.Id}", skill));
            response.EnsureSuccessStatusCode();

            var entity = await response.Content.ReadFromJsonAsync<Skill>();
            return entity;
        }

        public async Task DeleteAsync(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"{API_URI}/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}
