using CKTranslator.Core.Contracts.Services;
using CKTranslator.Core.Models;
using CKTranslator.Core.Parsing;

using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CKTranslator.Core.Services
{
    public class ModuleService
    {
        private readonly IModuleSettingsService settingsService;

        public ModuleService(IModuleSettingsService settingsService)
        {
            this.settingsService = settingsService;
        }

        public ICollection<Module> LoadModules()
        {
            ScriptParser parser = new();
            string[] fileNames = Directory.GetFiles(settingsService.ModsPath, "*.mod");

            var settings =
                (from string settingsLine in settingsService.ReadModuleSettings()
                 where settingsLine != null
                 let pos = settingsLine.IndexOf('"')
                 where pos >= 0
                 let parts = settingsLine[(pos + 1)..].Split('"')
                 select new
                 {
                     Name = settingsLine[..pos],
                     IsTranslated = ToBool(parts.ElementAtOrDefault(0)),
                     IsRecoded = ToBool(parts.ElementAtOrDefault(1))
                 })
                .ToDictionary(x => x.Name, x => (x.IsTranslated, x.IsRecoded));

            var modules = fileNames
                .Select(parser.ParseModule)
                .Append(CreateMainModule())
                .OfType<Module>()
                .ToDictionary(module => module.Name);

            foreach (Module module in modules.Values)
            {
                module.Dependencies = module.Dependencies
                    .Select(dependence =>
                    {
                        modules.TryGetValue(dependence.Name, out Module? result);
                        return result;
                    })
                    .OfType<Module>()
                    .ToArray();
                module.Path = System.IO.Path.Combine(settingsService.ModsPath, module.Path[4..]);
                if (!module.IsArchive)
                {
                    //module.HasScripts = module.GetModuleFiles()?.Count > 0;
                    module.IsBackuped = module.GetBackupFiles()?.Count > 0;

                    if (settings.TryGetValue(module.Name, out var values))
                    {
                        module.IsTranslated = values.IsTranslated;
                        module.IsRecoded = values.IsRecoded;
                    }
                }
            }

            return modules.Values
                .OrderBy(x => x.Name)
                .ToList();

            static bool ToBool(string? value, bool @default = false)
                => value switch
                {
                    "1" => true,
                    "0" => false,
                    _ => @default
                };
        }

        public void SaveModuleSettings(IList<Module> modules)
        {
            ICollection<string> settingLines = new List<string>();

            foreach (Module module in modules)
            {
                string settings = string.Join("\"",
                    module.Name,
                    ToIntString(module.IsTranslated),
                    ToIntString(module.IsRecoded)
                //ToIntString(moduleView.IsTranslationLoaded)
                );
                settingLines.Add(settings);
            }

            settingsService.SaveModuleSettings(settingLines);

            string ToIntString(bool value) => value ? "1" : "0";
        }

        private Module CreateMainModule()
        {
            Module mainModule = new()
            {
                IsArchive = false,
                Name = "Crusader Kings II",
                Path = settingsService.GamePath,
            };
            mainModule.IsBackuped = mainModule.GetBackupFiles().Count > 0;

            return mainModule;
        }
    }
}