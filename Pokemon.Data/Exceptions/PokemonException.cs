using System;

namespace Pokemon.Data.Exceptions
{
    public class PokemonException : Exception
    {
        public PokemonError PokemonError { get; set; }

        public PokemonException(PokemonError pokemonError)
        {
            PokemonError = pokemonError;
        }

        public PokemonException(PokemonError pokemonError, string message)
            : base(message)
        {
            PokemonError = pokemonError;
        }

        public PokemonException(PokemonError pokemonError, string message, Exception inner)
            : base(message, inner)
        {
            PokemonError = pokemonError;
        }
    }
}
