using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Pokemon.API.Profiles;
using Pokemon.Controllers;
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
                var mappingConfig = new MapperConfiguration(mc =>
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

            var controller = new PokemonController(loggerStub.Object, null, serviceStub.Object);

            // Act
            var result = await controller.GetPokemon(It.IsAny<string>());

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetPokemon_ReturnsExpectedItem()
        {
            // Arrange
            var expectedPokemon = CreateRandomPokemon();
            var expectedPokemonViewModel = CreateRandomPokemonViewModel();

            serviceStub
                .Setup(x => x.GetPokemonAsync(It.IsAny<string>()))
                .ReturnsAsync(expectedPokemon);
  
            var controller = new PokemonController(loggerStub.Object, _mapper, serviceStub.Object);

            // Act
            var result = await controller.GetPokemon(It.IsAny<string>());

            // Assert
            Assert.NotNull(result);
            var okObjectResult = (result.Result as OkObjectResult);
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
