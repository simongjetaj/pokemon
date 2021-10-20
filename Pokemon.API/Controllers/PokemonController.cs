using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Pokemon.Data.Models;
using Pokemon.Data.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pokemon.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PokemonController : ControllerBase
    {
        private readonly ILogger<PokemonController> _logger;
        private readonly PokeApiService _pokeApiService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="pokeApiService"></param>
        public PokemonController(ILogger<PokemonController> logger, PokeApiService pokeApiService)
        {
            _logger = logger;
            _pokeApiService = pokeApiService;
        }


        /// <summary>
        /// Given a Pokemon name, returns standard Pokemon description and additional information
        /// </summary>
        /// <returns>PokemonViewModel</returns>
        [Route("{name}")]
        [HttpGet]
        public async Task<ActionResult<PokemonViewModel>> Get(string name)
        {
            Data.Models.Pokemon pokemon = await _pokeApiService.GetResource(name);

            if (pokemon == null)
            {
                return NotFound();
            }

            PokemonViewModel pokemonViewModel = new() {
                Name = pokemon.Name,
                Description = _pokeApiService.GetFirstEnglishDescription(pokemon.Flavor_Text_Entries),
                IsLegendary = pokemon.IsLegendary,
                Habitat = pokemon.Habitat.Name,
            };

            return Ok(pokemonViewModel);
        }
    }
}
