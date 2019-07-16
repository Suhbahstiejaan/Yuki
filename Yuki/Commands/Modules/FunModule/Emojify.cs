using Qmmands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Yuki.Commands.Modules.FunModule
{
    public partial class FunModule
    {
        [Command("emojify")]
        public async Task EmojifyStringAsync([Remainder] string text)
        {
            text = text.ToLower();
            string value = "";

            string[] numbers = { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };

            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] >= 'a' && text[i] <= 'z')
                {
                    value += $":regional_indicator_{text[i]}:";
                }
                else
                {
                    if (text[i] == ' ')
                    {
                        value += "\t";
                    }
                    else if(text[i] == '!')
                    {
                        value += ":exclamation:";
                    }
                    else if (text[i] >= '0' && text[i] <= '9')
                    {
                        text += $":{numbers[text[i] - '0']}:";
                    }
                    else
                    {
                        value += text[i];
                    }
                }
            }
            //string.Join("\t", txt.Where(y => y != ' ').Select(x => ":regional_indicator_" + x + ":"))
            await ReplyAsync(value);
        }
    }
}
