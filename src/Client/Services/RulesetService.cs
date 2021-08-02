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
    public class RulesetService : IRulesetService
    {
        private const string RULESET_URI = "/api/rulesets";

        private HttpClient _httpClient;

        public RulesetService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Ruleset> GetByIdAsync(Guid id)
        {
            try
            {
                var ruleset = await _httpClient.GetFromJsonAsync<Ruleset>($"{RULESET_URI}/{id}");
                return ruleset;
            }
            catch(HttpRequestException httpRequestException)
            {
                if (httpRequestException.Message.Contains("404"))
                    throw new InvalidOperationException("Ruleset not found.");

                throw;
            }
        }

        public async Task<IEnumerable<Ruleset>> GetAllAsync()
        {
            var rulesets = await _httpClient.GetFromJsonAsync<List<Ruleset>>(RULESET_URI);
            return rulesets;
        }

        public async Task<Ruleset> AddAsync(Ruleset ruleset)
        {
            var response = await Task.Run(async () => await _httpClient.PostAsJsonAsync(RULESET_URI, ruleset));
            if(!response.IsSuccessStatusCode)
            {
                if(response.StatusCode == HttpStatusCode.PaymentRequired)
                    throw new SubscriptionException((await response.Content.ReadFromJsonAsync<ApiException>()).Message);

                response.EnsureSuccessStatusCode();
            }

            var createdRuleset = await response.Content.ReadFromJsonAsync<Ruleset>();
            return createdRuleset;
        }

        public async Task<Ruleset> ImportAsync(Guid templateId)
        {
            var response = await Task.Run(async () => await _httpClient.PostAsJsonAsync($"{RULESET_URI}/import", new { templateId }));
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.PaymentRequired)
                    throw new SubscriptionException((await response.Content.ReadFromJsonAsync<ApiException>()).Message);

                response.EnsureSuccessStatusCode();
            }


            var createdRuleset = await response.Content.ReadFromJsonAsync<Ruleset>();
            return createdRuleset;
        }

        public async Task UpdateAsync(Ruleset ruleset)
        {
            var response = await Task.Run(async () => await _httpClient.PutAsJsonAsync($"{RULESET_URI}/{ruleset.Id}", ruleset));
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteAsync(Guid rulesetId)
        {
            var response = await _httpClient.DeleteAsync($"{RULESET_URI}/{rulesetId}");
            response.EnsureSuccessStatusCode();
        }
    }
}
