using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using Yuki.Data.Objects.API.Pokemon;

namespace Yuki.API
{
    public static class PokeApi
    {
        public static readonly string apiRootUrl = "https://pokeapi.co/api/v2/";
        public static readonly string apiPokemon = apiRootUrl + "pokemon/";
        public static readonly string apiSpecies = apiRootUrl + "pokemon-species/";

        public static PokemonInfo GetPokemon(string pokemonName)
        {
            using(HttpClient client = new HttpClient())
            {
                using(StreamReader reader = new StreamReader(client.GetStreamAsync(apiPokemon + pokemonName).Result))
                {
                    return JsonConvert.DeserializeObject<PokemonInfo>(reader.ReadToEnd());
                }
            }
        }

        public static SpeciesInfo GetPokemonSpeciesInfo(string pokemon)
        {
            using (HttpClient client = new HttpClient())
            {
                using (StreamReader reader = new StreamReader(client.GetStreamAsync(apiSpecies + pokemon).Result))
                {
                    return JsonConvert.DeserializeObject<SpeciesInfo>(reader.ReadToEnd());
                }
            }
        }

        public static EvolutionInfo GetPokemonEvolutionInfo(string evoChainUrl)
        {
            using (HttpClient client = new HttpClient())
            {
                using (StreamReader reader = new StreamReader(client.GetStreamAsync(evoChainUrl).Result))
                {
                    return JsonConvert.DeserializeObject<EvolutionInfo>(reader.ReadToEnd());
                }
            }
        }
    }
}
