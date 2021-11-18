using System;
using System.Collections.Generic;
using System.Linq;

using CKTranslator.Core.Contracts.Services;
using CKTranslator.Core.Services;
using CKTranslator.ViewModels;

namespace CKTranslator.Services
{
    public class ObservableModuleService
    {
        private readonly ModuleService moduleService;
        private readonly IModuleSettingsService settingsService;

        public ObservableModuleService(IModuleSettingsService settingsService, ModuleService moduleService)
        {
            this.settingsService = settingsService;
            this.moduleService = moduleService;
        }

        public List<ModuleViewModel> GetModuleViewModels()
        {
            var settings =
                (from string settingsLine in settingsService.ReadModuleSettings()
                 where settingsLine != null
                 let pos = settingsLine.IndexOf('"')
                 where pos >= 0
                 let parts = settingsLine[(pos + 1)..].Split('"')
                 select new
                 {
                     Name = settingsLine[..pos],
                     IsSelected = ToBool(parts.ElementAtOrDefault(2))
                 })
                .ToDictionary(x => x.Name, x => x.IsSelected);

            return moduleService
                .LoadModules()
                .Select(module => new ModuleViewModel(module)
                {
                    IsSelected = settings.TryGetValue(module.Name, out bool isSelected) && isSelected
                })
                .ToList();

            static bool ToBool(string? value, bool @default = false)
                => value switch
                {
                    "1" => true,
                    "0" => false,
                    _ => @default
                };
        }

        public void SaveSettings(IList<ModuleViewModel> moduleViews)
        {
            ICollection<string> settingLines = new List<string>();

            foreach (ModuleViewModel moduleView in moduleViews)
            {
                string settings = string.Join("\"",
                    moduleView.Module.Name,
                    ToIntString(moduleView.IsTranslated),
                    ToIntString(moduleView.IsRecoded),
                    ToIntString(moduleView.IsSelected)
                //ToIntString(moduleView.IsTranslationLoaded)
                );
                settingLines.Add(settings);
            }

            settingsService.SaveModuleSettings(settingLines);

            string ToIntString(bool value) => value ? "1" : "0";
        }

        //private IList<ModuleViewModel> SortByDependencies(IList<ModuleViewModel> modules)
        //{
        //    return modules
        //        .Select(moduleView => moduleView.Module)
        //        .OrderByTopology(Module => Module.Dependencies, true)
        //        .Select(Module => modules.First(modView => modView.Module == Module))
        //        .ToArray();
        //}
    }
}