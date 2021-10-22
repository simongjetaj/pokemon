using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pokemon.Data.Exceptions;
using Pokemon.Data.Models;
using Pokemon.Data.Services;
using System.Threading.Tasks;

namespace Pokemon.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PokemonController : ControllerBase
    {
        private readonly ILogger<PokemonController> _logger;
        private readonly IPokemonApiService _pokeApiService;
        private readonly IMapper _mapper;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="pokeApiService"></param>
        public PokemonController(ILogger<PokemonController> logger, IMapper mapper, IPokemonApiService pokeApiService)
        {
            _logger = logger;
            _mapper = mapper;

            _pokeApiService = pokeApiService;
        }


        /// <summary>
        /// Given a Pokemon name, returns standard Pokemon description and additional information
        /// </summary>
        /// <returns>PokemonViewModel</returns>
        [Route("{pokemonName}")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PokemonViewModel>> GetPokemon(string pokemonName)
        {
            Data.Models.Pokemon pokemon = await _pokeApiService.GetPokemonAsync(pokemonName);

            if (pokemon == null)
            {
                return NotFound();
            }

            PokemonViewModel pokemonViewModel = _mapper.Map<PokemonViewModel>(pokemon);

            return Ok(pokemonViewModel);
        }


        /// <summary>
        /// Given a Pokemon name, returns translated Pokemon description and other basic information using some rules
        /// </summary>
        /// <returns>PokemonViewModel</returns>
        [Route("translated/{pokemonName}")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PokemonViewModel>> GetTranslatedPokemon(string pokemonName)
        {
            Data.Models.Pokemon translatedPokemon = await _pokeApiService.GetTranslatedPokemonAsync(pokemonName);

            if (translatedPokemon == null)
            {
                return NotFound();
            }

            PokemonViewModel pokemonViewModel = _mapper.Map<PokemonViewModel>(translatedPokemon);

            return Ok(pokemonViewModel);
        }
    }
}
