using Discord.Commands;
using System.Threading.Tasks;

namespace Yuki.Modules.UserModule
{
    public partial class User
    {
        [Command("reverse")]
        public async Task ReverseTextAsync([Remainder] string txt)
        {
            string[] stringList = txt.Split(new char[] { ' ' });

            for (int i = 0; i < stringList.Length; i++)
            {
                string reversedSubstring = "";

                for (int j = stringList[i].Length; j-- > 0;)
                {
                    reversedSubstring += stringList[i][j];
                }

                stringList[i] = reversedSubstring;
            }

            await ReplyAsync(string.Join(' ', stringList));
        }
    }
}
