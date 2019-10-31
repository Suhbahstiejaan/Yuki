using System;
using System.Collections.Generic;
using System.Text;

namespace Yuki.Services.Database
{
    public class Patreon
    {
        public static bool IsPatron(ulong userId)
        {
            return UserSettings.IsPatron(userId);
        }
    }
}
