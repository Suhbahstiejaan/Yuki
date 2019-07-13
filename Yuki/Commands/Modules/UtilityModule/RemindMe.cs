using Qmmands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Yuki.Data.Objects;
using Yuki.Data.Objects.Database;
using Yuki.Extensions;
using Yuki.Services;
using Yuki.Services.Database;

namespace Yuki.Commands.Modules.UtilityModule
{
    public partial class UtilityModule
    {
        [Command("remindme", "remind")]
        public async Task RemindMeAsync([Remainder] string reminder)
        {
            string[] data = Regex.Split(reminder, @"\s*[in]\s*").Where(str => !string.IsNullOrWhiteSpace(str)).ToArray();

            Console.WriteLine(string.Join("_", data));

            if(data.Length < 2 || data.Length > 2)
            {
                await ReplyAsync(Language.GetString("remindme_incorrect_response_string"));
                return;
            }

            DateTime now = DateTime.UtcNow;
            DateTime date = data[1].ToDateTime();

            if (data == default)
            {
                await ReplyAsync(Language.GetString("poll_create_deadline_invalid"));
                return;
            }
            else if ((date.TimeOfDay.TotalDays / 7) > 2)
            {
                await ReplyAsync(Language.GetString("poll_create_deadline_long"));
                return;
            }
            else if((date - now).TotalSeconds < 60)
            {
                await ReplyAsync(Language.GetString("remindme_datetime_short"));
            }

            if(data[0].Length > 1000)
            {
                data[0] = data[0].Substring(0, 1000) + "...";
            }

            try
            {
                UserSettings.AddReminder(new YukiReminder()
                {
                    Message = data[0],
                    Time = date,
                    AuthorId = Context.User.Id,
                });

                await ReplyAsync(Language.GetString("remindme_success").Replace("%user%", Context.User.Username).Replace("%reminder%", data[0]));
            }
            catch(Exception e)
            {
                LoggingService.Write(LogLevel.Error, e);
                await ReplyAsync("error_occurred");
            }
        }
    }
}
