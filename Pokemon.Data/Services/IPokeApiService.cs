using Pokemon.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pokemon.Data.Services
{
    public interface IPokeApiService
    {
        public Task<Models.Pokemon> GetResource(string name);
        string GetFirstEnglishDescription(List<FlavorText> flavor_text_entries);
    }
}