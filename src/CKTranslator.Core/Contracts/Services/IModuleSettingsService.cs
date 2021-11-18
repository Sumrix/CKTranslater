using System.Collections.Generic;

namespace CKTranslator.Core.Contracts.Services
{
    public interface IModuleSettingsService
    {
        string GamePath { get; set; }

        string ModsPath { get; set; }

        List<string> ReadModuleSettings();

        void SaveModuleSettings(ICollection<string> rusModulesSettings);
    }
}