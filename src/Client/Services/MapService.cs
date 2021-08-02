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
    public class MapService : IMapService
    {
        private const string MAP_URI = "/api/maps";

        private HttpClient _httpClient;

        public MapService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Map> GetByIdAsync(Guid id)
        {
            try
            {
                var map = await _httpClient.GetFromJsonAsync<Map>($"{MAP_URI}/{id}");
                return map;
            }
            catch(HttpRequestException httpRequestException)
            {
                if (httpRequestException.Message.Contains("404"))
                    throw new InvalidOperationException("Map not found.");

                throw;
            }
        }

        public async Task<IEnumerable<Map>> GetAllAsync()
        {
            var maps = await _httpClient.GetFromJsonAsync<List<Map>>(MAP_URI);
            return maps;
        }

        public async Task<IEnumerable<Map>> GetAllByRulesetIdAsync(Guid rulesetId)
        {
            var maps = await _httpClient.GetFromJsonAsync<List<Map>>($"/api/rulesets/{rulesetId}/maps");
            return maps;
        }

        public async Task<Map> AddAsync(Guid rulesetId, Map map)
        {
            var response = await Task.Run(async () => await _httpClient.PostAsJsonAsync($"/api/rulesets/{rulesetId}/maps", map));
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.PaymentRequired)
                    throw new SubscriptionException((await response.Content.ReadFromJsonAsync<ApiException>()).Message);

                response.EnsureSuccessStatusCode();
            }

            var createdMap = await response.Content.ReadFromJsonAsync<Map>();
            return createdMap;
        }

        public async Task UpdateAsync(Guid rulesetId, Map map)
        {
            // Serialization appears to not be ran async.
            var response = await Task.Run(async () => await _httpClient.PutAsJsonAsync($"/api/rulesets/{rulesetId}/maps/{map.Id}", map));
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteAsync(Guid mapId)
        {
            var response = await _httpClient.DeleteAsync($"{MAP_URI}/{mapId}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<string> SetImageAsync(Guid mapId, string fileName, byte[] image)
        {
            var multiPartContent = new MultipartFormDataContent();
            multiPartContent.Add(new ByteArrayContent(image), "file", fileName);

            var response = await _httpClient.PostAsync($"{MAP_URI}/{mapId}/image", multiPartContent);
            response.EnsureSuccessStatusCode();

            var url = await response.Content.ReadAsStringAsync();
            return url;
        }
    }
}
