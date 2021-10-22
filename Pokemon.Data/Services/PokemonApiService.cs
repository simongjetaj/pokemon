using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Pokemon.Data.Models;
using System.Text;
using Pokemon.Data.Exceptions;

namespace Pokemon.Data.Services
{
    public class PokemonApiService : IPokemonApiService
    {
        private readonly ILogger<PokemonApiService> _logger;
        private readonly HttpClient _httpClient;
        public IConfiguration _configuration;

        public PokemonApiService(ILogger<PokemonApiService> logger, HttpClient httpClient, IConfiguration configuration)
        {
            _logger = logger;
            _httpClient = httpClient;
            _configuration = configuration;
        }

        /// <summary>
        /// Get a resource by name
        /// </summary>
        /// <returns>Pokemon</returns>
        public async Task<Models.Pokemon> GetPokemonAsync(string pokemonName)
        {
            string url = $"{_configuration["Pokemon:ApiBaseUrl"]}/pokemon-species/{pokemonName}";
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
        /// <returns>Translation</returns>
        private async Task<Translation> GetTranslatedPokemonDescriptionAsync(string textToBeTranslated, TranslationType translationType)
        {
            string url = $"{_configuration["FunTranslations:ApiBaseUrl"]}/translate/{translationType.Value}";
            string json = JsonConvert.SerializeObject(new { text = textToBeTranslated });

            StringContent data = new(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PostAsync(url, data);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            string result = await response.Content.ReadAsStringAsync();
            Translation translation = JsonConvert.DeserializeObject<Translation>(result);

            return translation;
        }


        /// <summary>
        /// Get Pokemon with translated description
        /// </summary>
        /// <param name="pokemonName"></param>
        /// <returns>Pokemon</returns>
        public async Task<Models.Pokemon> GetTranslatedPokemonAsync(string pokemonName)
        {
            Models.Pokemon pokemon = await GetPokemonAsync(pokemonName);

            if (pokemon == null)
            {
                return null;
            }

            string descriptionToBeTranslated = GetFirstEnglishDescription(pokemon.Flavor_Text_Entries);
            Translation translation = new();

            try
            {
                if (pokemon.Habitat.Name == "cave" && pokemon.IsLegendary)
                {
                    translation = await GetTranslatedPokemonDescriptionAsync(descriptionToBeTranslated, TranslationType.Yoda);
                }
                else
                {
                    translation = await GetTranslatedPokemonDescriptionAsync(descriptionToBeTranslated, TranslationType.Shakespeare);
                }
            }
            catch
            {
                translation.Contents.Translated = descriptionToBeTranslated;
            }

            pokemon.Flavor_Text_Entries.First().Flavor_Text = translation.Contents.Translated;

            return pokemon;
        }


        /// <summary>
        /// Get first english description
        /// </summary>
        /// <param name="flavor_text_entries"></param>
        /// <returns>string</returns>
        public string GetFirstEnglishDescription(List<FlavorText> flavor_text_entries)
        {
            if (!flavor_text_entries.Any())
            {
                throw new PokemonException(PokemonError.BadRequest, $"Flavor text entries must not be empty.");
            }

            return flavor_text_entries.First(x => x.Language.Name == "en").Flavor_Text;
        }
    }
}
