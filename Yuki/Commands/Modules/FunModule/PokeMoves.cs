using Qmmands;
using System.Linq;
using System.Threading.Tasks;
using Yuki.API;
using Yuki.Data.Objects.API.Pokemon;

namespace Yuki.Commands.Modules.FunModule
{
    public partial class FunModule
    {
        /*[Command("pokemoves", "pmoves")]
        public async Task PokeMovesAsync(string pokemon)
        {
            PokemonInfo pokeInf = PokeApi.GetPokemon(pokemon);

            await ReplyAsync(string.Join(", ", pokeInf.moves.Select(move => move.move.name)));
        }*/
    }
}
