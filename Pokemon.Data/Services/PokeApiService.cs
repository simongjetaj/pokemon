using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Pokemon.Data.Models;

namespace Pokemon.Data.Services
{
    public class PokeApiService : IPokeApiService
    {
        private readonly ILogger<PokeApiService> _logger;
        private readonly HttpClient _httpClient;
        public IConfiguration _configuration;

        public PokeApiService(ILogger<PokeApiService> logger, HttpClient httpClient, IConfiguration configuration)
        {
            _logger = logger;
            _httpClient = httpClient;
            _configuration = configuration;
        }

        /// <summary>
        /// Get a resource by name
        /// </summary>
        /// <returns>JObject</returns>
        public async Task<Models.Pokemon> GetResource(string name)
        {
            string url = $"{_configuration["Pokemon:ApiUrl"]}/pokemon-species/{name}";

            HttpResponseMessage response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            string result = await response.Content.ReadAsStringAsync();
            Models.Pokemon pokemon = JsonConvert.DeserializeObject<Models.Pokemon>(result);

            return pokemon;
        }


        /// <summary>
        /// Get translated Pokemon description
        /// Given a Pokemon name, return translated Pokemon description and other basic information following some rules
        /// </summary>
        /// <returns>JObject</returns>
        public async Task<JObject> GetTranslatedPokemonDescription(string name)
        {
            string url = $"{_configuration["Pokemon:ApiUrl"]}/pokemon-species/{name}";

            HttpResponseMessage response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            string result = await response.Content.ReadAsStringAsync();
            JObject json = JObject.Parse(result);

            return json;
        }


        /// <summary>
        /// Get first english description
        /// </summary>
        /// <param name="flavor_text_entries"></param>
        /// <returns>string</returns>
        public string GetFirstEnglishDescription(List<FlavorText> flavor_text_entries)
        {
            return flavor_text_entries.FirstOrDefault(x => x.Language.Name == "en")?.Flavor_Text;
        }
    }
}
