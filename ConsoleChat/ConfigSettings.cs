using Microsoft.Extensions.Configuration;

namespace ConsoleChat
{
    public sealed class Settings
    {
        public int ServerPort { get; set; }
        public string ServerIpAddress { get; set; }
        public string NetworkId { get; set; }
    }
    internal class ConfigSettings
    {
        private const string SettingsKey = "Settings";
        static ConfigSettings()
        {
            var configuration = GetConfigSettings();
            AppSettings = configuration.GetSection(SettingsKey).Get<Settings>() ?? new Settings();
        }
        public static Settings AppSettings { get; }
        static IConfiguration GetConfigSettings()
        {
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            return builder.Build();
        }
    }
}
