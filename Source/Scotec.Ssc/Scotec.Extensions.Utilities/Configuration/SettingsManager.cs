#if NET8_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.Loader;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Scotec.Extensions.Utilities.Strings;

namespace Scotec.Extensions.Utilities.Configuration
{
    public class SettingsManager
    {
        public enum Environment
        {
            [SuppressMessage("ReSharper", "InconsistentNaming")]
            DOTNET_ENVIRANMENT,

            [SuppressMessage("ReSharper", "InconsistentNaming")]
            ASPNETCORE_ENVIRONMENT
        }

        private readonly ILogger<SettingsManager> _logger;

        private readonly Dictionary<string, Type> _optionsTypes;
        private readonly Dictionary<string, object> _settings;

        public SettingsManager(IOptions<SettingsManagerOptions> options, ILogger<SettingsManager> logger)
        {
            _logger = logger;
            _optionsTypes = GetOptionTypes();

            Options = options.Value;

            if (string.IsNullOrWhiteSpace(Options.SettingsFile))
            {
                const string message = "Settings file path is not specified in the options.";
                _logger.LogCritical(message);
                throw new InvalidOperationException(message);
            }

            //var settings = LoadSettings(settingsFilePath, s_optionsTypes, null);
            //SaveSettings(settingsFilePath, settings, null);

            _settings = LoadSettings(options.Value.SettingsFile, _optionsTypes);
        }

        public SettingsManagerOptions Options { get; }

        internal static Dictionary<string, Type> GetOptionTypes()
        {
            var assemblies = AssemblyLoadContext.GetLoadContext(typeof(SettingsManager).Assembly)!.Assemblies;

            return GetOptionTypes(assemblies);
        }

        internal static Dictionary<string, Type> GetOptionTypes(IEnumerable<Assembly> assemblies)
        {
            // Search for types with the SettingsSectionAttribute in all assemblies
            return assemblies
                   .SelectMany(assembly => assembly.GetTypes())
                   .Where(type => type.GetCustomAttribute<SettingsSectionAttribute>() != null)
                   .Select(type => (Type: type, type.GetCustomAttribute<SettingsSectionAttribute>()!.SectionName))
                   .ToDictionary(attributedType => attributedType.SectionName, attributedType => attributedType.Type);
        }

        public void SaveSettings(IEnumerable<object> settings)
        {
            if (string.IsNullOrWhiteSpace(Options.SettingsFile))
            {
                const string message = "Settings file path is not specified in the options.";
                _logger.LogCritical(message);
                throw new InvalidOperationException(message);
            }

            var result = settings.ToDictionary(
                setting => _optionsTypes!.FirstOrDefault(pair => pair.Value == setting.GetType()).Key,
                setting => setting);

            SaveSettings(Options.SettingsFile, result);
        }

        private void SaveSettings(string settingsFile, Dictionary<string, object> settings)
        {
            var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });

            var file = settingsFile.ExpandVariables();
            var path = Path.GetDirectoryName(file);

            _logger.LogInformation($"Saving settings to '{file}'.");
            // Create the directory if it does not exist
            if (!Directory.Exists(path))
            {
                _logger.LogInformation($"Creating directory '{path}'.");
                Directory.CreateDirectory(Path.GetDirectoryName(file)!);
            }

            File.WriteAllText(file, json);
        }

        private Dictionary<string, object> LoadSettings(string settingsFile, IDictionary<string, Type> optionTypes)
        {
            var file = settingsFile.ExpandVariables();

            // Creates an empty JSON object, which serves as a default if the settings file does not exist.
            var json = "{}";

            if (File.Exists(file))
            {
                _logger.LogInformation($"Reading settings from file '{file}'.");
                json = File.ReadAllText(file);
            }
            else
            {
                _logger.LogInformation($"Settings file '{file}' does not exist. Default settings will be used.");
            }

            var rawSettings = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json) ?? new Dictionary<string, JsonElement>();
            var settings = new Dictionary<string, object>();

            foreach (var (sectionName, optionType) in optionTypes)
            {
                settings[sectionName] = rawSettings.TryGetValue(sectionName, out var jsonElement)
                    ? JsonSerializer.Deserialize(jsonElement.GetRawText(), optionType) ?? Activator.CreateInstance(optionType)!
                    : Activator.CreateInstance(optionType)!;
            }

            return settings;
        }
    }
}

#endif
