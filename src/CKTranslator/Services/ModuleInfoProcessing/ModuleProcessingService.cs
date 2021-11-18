//using CKTranslator.Core.Models;
//using CKTranslator.Core.Processing;
//using CKTranslator.ViewModels;

//using System.Collections.Generic;
//using System.Linq;

//namespace CKTranslator.Services
//{
//    public class ModuleProcessingService
//    {
//        private ObservableModuleProcessor? currentProcessor;

//        public void Backup(ICollection<ModuleViewModel> modules)
//        {
//            currentProcessor = new ModuleProcessor
//            {
//                CreateBackupProcess(modules)
//            };
//            currentProcessor.Run();
//        }

//        public void Recode(ICollection<ModuleViewModel> modules)
//        {
//            currentProcessor = new ModuleProcessor
//            {
//                CreateBackupProcess(modules),
//                CreateRecodeProcess(modules)
//            };
//            currentProcessor.Run();
//        }

//        public void Restore(ICollection<ModuleViewModel> modules)
//        {
//            List<Module> originalModules = new();
//            foreach (var moduleView in modules)
//            {
//                var originalModule = moduleView.Module;
//                originalModules.Add(originalModule);
//                moduleView.Module = new Module
//                {
//                    Name = originalModule.Name,
//                    Path = originalModule.Path,
//                };
//            }

//            currentProcessor = new ModuleProcessor
//            {
//                CreateRestoreProcess(modules)
//            };
//            currentProcessor.Run();

//            foreach (var (moduleView, originalModule) in modules.Zip(originalModules))
//            {
//                moduleView.Module = originalModule;
//            }
//        }

//        public void Stop()
//        {
//            currentProcessor?.Stop();
//        }

//        public void Translate(ICollection<ModuleViewModel> modules)
//        {
//            currentProcessor = new ModuleProcessor
//            {
//                CreateBackupProcess(modules),
//                CreateRecodeProcess(modules),
//                CreateTranslateProcess(modules, new())
//            };
//            currentProcessor.Run();
//        }

//        private ModuleProcess CreateBackupProcess(ICollection<ModuleViewModel> modules)
//        {
//            return new ModuleProcess
//            {
//                DocumentProcessor = Backuping.Backup,
//                Modules = modules,
//                ModuleProcessedAction = (_, e) => e.Module.IsBackuped = true,
//                RunCondition = mod => !mod.IsBackuped
//            };
//        }

//        private ModuleProcess CreateRecodeProcess(ICollection<ModuleViewModel> modules)
//        {
//            return new ModuleProcess
//            {
//                DocumentProcessor = FileRecoder.Recode,
//                Modules = modules,
//                ModuleProcessedAction = (_, e) => e.Module.IsRecoded = true,
//                RunCondition = mod => !mod.IsRecoded
//            };
//        }

//        private ModuleProcess CreateRestoreProcess(ICollection<ModuleViewModel> modules)
//        {
//            return new ModuleProcess
//            {
//                DocumentProcessor = Backuping.Restore,
//                Modules = modules,
//                ModuleProcessedAction = (_, e) =>
//                {
//                    e.Module.IsTranslated = false;
//                    e.Module.IsRecoded = false;
//                }
//            };
//        }

//        private ModuleProcess CreateTranslateProcess(ICollection<ModuleViewModel> modules, Translator translator)
//        {
//            return new ModuleProcess
//            {
//                DocumentProcessor = translator.TranslateScript,
//                Modules = modules,
//                ModuleProcessedAction = (_, e) => e.Module.IsTranslated = true
//            };
//        }
//    }
//}