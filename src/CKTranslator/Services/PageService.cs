﻿using System;
using System.Collections.Generic;
using System.Linq;

using CKTranslator.Contracts.Services;
using CKTranslator.ViewModels;
using CKTranslator.Views;

using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;

namespace CKTranslator.Services
{
    public class PageService : IPageService
    {
        private readonly Dictionary<string, Type> _pages = new();

        public PageService()
        {
            Configure<GeneralViewModel, MainPage>();
            Configure<ModulesViewModel, ModulesPage>();
            Configure<DictionaryViewModel, DictionaryPage>();
            //Configure<SettingsViewModel, SettingsPage>();
        }

        public Type GetPageType(string key)
        {
            Type? pageType;
            lock (_pages)
            {
                if (!_pages.TryGetValue(key, out pageType))
                {
                    throw new ArgumentException($"Page not found: {key}. Did you forget to call PageService.Configure?");
                }
            }

            return pageType;
        }

        private void Configure<VM, V>()
            where VM : ObservableObject
            where V : Page
        {
            lock (_pages)
            {
                var key = typeof(VM).FullName;
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key));
                }

                if (_pages.ContainsKey(key))
                {
                    throw new ArgumentException($"The key {key} is already configured in PageService");
                }

                var type = typeof(V);
                if (_pages.Any(p => p.Value == type))
                {
                    throw new ArgumentException($"This type is already configured with key {_pages.First(p => p.Value == type).Key}");
                }

                _pages.Add(key, type);
            }
        }
    }
}
