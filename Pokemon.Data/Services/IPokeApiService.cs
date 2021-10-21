using Pokemon.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pokemon.Data.Services
{
    public interface IPokeApiService
    {
        Task<Models.Pokemon> GetPokemonAsync(string name);
        Task<Models.Pokemon> GetTranslatedPokemonAsync(string pokemonName);
        string GetFirstEnglishDescription(List<FlavorText> flavor_text_entries);
    }
}