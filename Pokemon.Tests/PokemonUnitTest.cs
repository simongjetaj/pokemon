using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Pokemon.API.Profiles;
using Pokemon.Controllers;
using Pokemon.Data.Exceptions;
using Pokemon.Data.Models;
using Pokemon.Data.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Pokemon.Tests
{
    public class PokemonUnitTest
    {
        private readonly Mock<IPokemonApiService> serviceStub = new();
        private readonly Mock<ILogger<PokemonController>> loggerStub = new();
        private static IMapper _mapper;

        public PokemonUnitTest()
        {
            if (_mapper == null)
            {
                MapperConfiguration mappingConfig = new (mc =>
                {
                    mc.AddProfile(new PokemonProfile());
                });
                IMapper mapper = mappingConfig.CreateMapper();
                _mapper = mapper;
            }
        }

        [Fact]
        public async Task GetPokemon_ReturnsNotFound()
        {
            // Arrange
            serviceStub
                .Setup(x => x.GetPokemonAsync(It.IsAny<string>()))
                .ReturnsAsync((Data.Models.Pokemon)null);

            PokemonController controller = new (loggerStub.Object, _mapper, serviceStub.Object);

            // Assert
            Task<PokemonException> ex = Assert.ThrowsAsync<PokemonException>(async () => await controller.GetPokemon(It.IsAny<string>()));
            Assert.Equal((await ex).PokemonError.StatusCode, StatusCodes.Status404NotFound);
        }

        [Fact]
        public async Task GetPokemon_ReturnsExpectedItem()
        {
            // Arrange
            Data.Models.Pokemon expectedPokemon = CreateRandomPokemon();
            PokemonViewModel expectedPokemonViewModel = CreateRandomPokemonViewModel();

            serviceStub
                .Setup(x => x.GetPokemonAsync(It.IsAny<string>()))
                .ReturnsAsync(expectedPokemon);

            PokemonController controller = new (loggerStub.Object, _mapper, serviceStub.Object);

            // Act
            ActionResult<PokemonViewModel> result = await controller.GetPokemon(It.IsAny<string>());

            // Assert
            Assert.NotNull(result);
            OkObjectResult okObjectResult = (result.Result as OkObjectResult);
            PokemonViewModel pokemonViewModel = (PokemonViewModel)okObjectResult.Value;
            Assert.IsType<PokemonViewModel>(pokemonViewModel);
            Assert.Equal(okObjectResult.StatusCode, StatusCodes.Status200OK);
            Assert.Equal(expectedPokemon.Name, pokemonViewModel.Name);
            Assert.Equal(expectedPokemon.Flavor_Text_Entries[0].Flavor_Text, pokemonViewModel.Description);
            Assert.Equal(expectedPokemon.Habitat.Name, pokemonViewModel.Habitat);
            Assert.Equal(expectedPokemon.IsLegendary, pokemonViewModel.IsLegendary);
        }

        private static Data.Models.Pokemon CreateRandomPokemon()
        {
            return new Data.Models.Pokemon ()
            {
                Name = "mewtwo",
                Flavor_Text_Entries = new List<FlavorText>
                {
                    new FlavorText()
                    {
                        Flavor_Text = "It was created by\na scientist after\nyears of horrific\fgene splicing and\nDNA engineering\nexperiments",
                        Language = new Language ()
                        {
                            Name = "en"
                        }
                    }  
                },
                Habitat = new Habitat { 
                    Name = "rare"
                },
                IsLegendary = false
            };
        }

        private static PokemonViewModel CreateRandomPokemonViewModel()
        {
            return new PokemonViewModel()
            {
                Name = "mewtwo",
                Description = "It was created by\na scientist after\nyears of horrific\fgene splicing and\nDNA engineering\nexperiments",
                Habitat = "rare",
                IsLegendary = false
            };
        }
    }
}
