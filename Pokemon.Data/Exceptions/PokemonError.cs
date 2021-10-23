using System.Net;

namespace Pokemon.Data.Exceptions
{
    public class PokemonError
    {
        private PokemonError(int statusCode, string errorCode) 
        {
            StatusCode = statusCode;
            ErrorCode = errorCode; 
        }

        public int StatusCode { get; private set; }
        public string ErrorCode { get; private set; }

        public static PokemonError BadRequest
        { 
            get { 
                return new PokemonError((int)HttpStatusCode.BadRequest, "bad_request"); 
            } 
        }

        public static PokemonError NotFoundRequest
        {
            get
            {
                return new PokemonError((int)HttpStatusCode.NotFound, "not_found_request");
            }
        }
    }
}
