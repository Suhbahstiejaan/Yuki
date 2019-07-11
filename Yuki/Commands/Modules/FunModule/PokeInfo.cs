using Discord;
using Qmmands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yuki.API;
using Yuki.Data.Objects.API.Pokemon;

namespace Yuki.Commands.Modules.FunModule
{
    public partial class FunModule
    {
        [Command("pokeinfo", "pinfo")]
        public async Task PokeInfoAsync(string pokemonName)
        {
            PokemonInfo pokeInf = PokeApi.GetPokemon(pokemonName);

            List<string> evolutions = new List<string>();

            try
            {
                Chain evoChain = PokeApi.GetPokemonEvolutionInfo(PokeApi.GetPokemonSpeciesInfo(pokemonName).evolution_chain.url)
                                    .chain;

                evolutions.Add(evoChain.species.name);

                foreach (var evolution in evoChain.evolves_to)
                {
                    if (!evolutions.Contains(evolution.species.name))
                    {
                        evolutions.Add(evolution.species.name);
                    }

                    foreach (var evo in evolution.evolves_to)
                    {
                        if (!evolutions.Contains(evo.species.name))
                        {
                            evolutions.Add(evo.species.name);
                        }
                    }
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }

            EmbedBuilder embed = new EmbedBuilder()
                .WithAuthor(pokeInf.name)
                .WithThumbnailUrl(pokeInf.sprites.front_default);
            embed.WithDescription(PokeApi.GetPokemonSpeciesInfo(pokemonName).flavor_text_entries.Find(entry => entry.language.name == "en").flavor_text);
            embed.AddField(Language.GetString("pokemon_weight"), pokeInf.weight.ToString(), true);
            embed.AddField(Language.GetString("pokemon_height"), pokeInf.height, true);
            embed.AddField(Language.GetString("pokemon_base_exp"), pokeInf.base_experience, true);
            embed.AddField(Language.GetString("pokemon_abilities"), string.Join(", ", pokeInf.abilities.Select(ability => ability.ability.name)), true);
            embed.AddField(Language.GetString("pokemon_types"), string.Join(", ", pokeInf.types.Select(type => type.type.name)), true);
            embed.AddField(Language.GetString("pokemon_evolution_chain"), string.Join(" -> ", evolutions), true);

            await ReplyAsync(embed);
        }
    }
}
