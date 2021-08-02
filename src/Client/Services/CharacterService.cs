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
    public class CharacterService : ICharacterService
    {
        private const string API_URI = "/api/characters";

        private HttpClient _httpClient;

        public CharacterService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Character> GetByIdAsync(Guid id)
        {
            try
            {
                var character = await _httpClient.GetFromJsonAsync<Character>($"{API_URI}/{id}");
                return character;
            }
            catch(HttpRequestException httpRequestException)
            {
                if (httpRequestException.Message.Contains("404"))
                    throw new InvalidOperationException("Character not found.");

                throw;
            }
        }

        public async Task<IEnumerable<Character>> GetAllByRulesetIdAsync(Guid rulesetId)
        {
            var characters = await _httpClient.GetFromJsonAsync<List<Character>>($"/api/rulesets/{rulesetId}/characters");
            return characters;
        }

        public async Task<Character> AddAsync(Guid rulesetId, Character character)
        {
            var response = await Task.Run(async () => await _httpClient.PostAsJsonAsync($"/api/rulesets/{rulesetId}/characters", character));
            response.EnsureSuccessStatusCode();

            var createdCharacter = await response.Content.ReadFromJsonAsync<Character>();
            return createdCharacter;
        }

        public async Task<Character> UpdateAsync(Guid rulesetId, Character character)
        {
            var response = await Task.Run(async () => await _httpClient.PutAsJsonAsync($"/api/rulesets/{rulesetId}/characters/{character.Id}", character));
            response.EnsureSuccessStatusCode();

            var entity = await response.Content.ReadFromJsonAsync<Character>();
            return entity;
        }

        public async Task DeleteAsync(Guid characterId)
        {
            var response = await _httpClient.DeleteAsync($"{API_URI}/{characterId}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<string> SetImageAsync(Guid characterId, string fileName, byte[] image)
        {
            var multiPartContent = new MultipartFormDataContent();
            multiPartContent.Add(new ByteArrayContent(image), "file", fileName);

            var response = await _httpClient.PostAsync($"{API_URI}/{characterId}/image", multiPartContent);
            response.EnsureSuccessStatusCode();

            var url = await response.Content.ReadAsStringAsync();
            return url;
        }

        public async Task<string> CopyImageAsync(Guid sourceCharacterId, Guid targetCharacterId)
        {
            var response = await _httpClient.PostAsJsonAsync<CopyImageRequest>($"{API_URI}/{targetCharacterId}/copyimage", 
                new CopyImageRequest
                {
                    SourceCharacterId = sourceCharacterId,
                });

            response.EnsureSuccessStatusCode();

            var url = await response.Content.ReadAsStringAsync();
            return url;
        }
    }
}
