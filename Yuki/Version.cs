using Discord;

namespace Yuki
{
    public static class Version
    {
        public static int Major  { get; } = 1;
        public static int Minor  { get; } = 6;
        public static int Hotfix { get; } = 0;
        public static int Patch  { get; } = 0;

        public static ReleaseType ReleaseType { get; } = ReleaseType.Development;

        public static string DiscordNetVersion { get; } = DiscordConfig.Version;

        public static string Get()
        {
            return $"{Major}.{Minor}.{Hotfix}.{Patch}";
        }

        public static string GetFull()
        {
            return Get() + $"-{ReleaseType}";
        }

        public static new string ToString()
        {
            return GetFull() + $" | Discord.Net {DiscordNetVersion}";
        }
    }

    public enum ReleaseType
    {
        Development,
        PreRelease,
        Release
    }
}
