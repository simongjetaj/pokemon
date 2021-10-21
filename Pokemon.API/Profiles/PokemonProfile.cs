using AutoMapper;
using Pokemon.Data.Models;
using System.Linq;

namespace Pokemon.API.Profiles
{
    public class PokemonProfile : Profile
    {
        public PokemonProfile()
        {
            CreateMap<Data.Models.Pokemon, PokemonViewModel>()
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Flavor_Text_Entries.First(x => x.Language.Name == "en").Flavor_Text))
                .ForMember(dest => dest.Habitat, opt => opt.MapFrom(src => src.Habitat.Name));
        }
    }
}
