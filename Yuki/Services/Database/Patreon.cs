using LiteDB;
using System.Linq;
using Yuki.Data.Objects.Database;

namespace Yuki.Services.Database
{
    public class Patreon
    {
        private const string collection = "patreon";

        public static bool IsPatron(ulong userId)
        {
            return UserSettings.IsPatron(userId);
        }

        public static void AddCommand(PatronCommand command)
        {
            using (LiteDatabase db = new LiteDatabase(FileDirectories.SettingsDB))
            {
                ILiteCollection<PatronCommand> commands = db.GetCollection<PatronCommand>(collection);

                if (!commands.FindAll().Any(cmd => cmd.UserId == command.UserId))
                {
                    commands.Insert(command);
                }
                else
                {
                    commands.Update(command);
                }
            }
        }

        public static PatronCommand GetCommand(ulong userId, string name)
        {
            using (LiteDatabase db = new LiteDatabase(FileDirectories.SettingsDB))
            {
                ILiteCollection<PatronCommand> commands = db.GetCollection<PatronCommand>(collection);

                if (!commands.FindAll().Any(cmd => cmd.UserId == userId))
                {
                    return default;
                }
                else
                {
                    return commands.FindAll().FirstOrDefault(cmd => cmd.UserId == userId && cmd.Name.ToLower() == name.ToLower());
                }
            }
        }
    }
}
