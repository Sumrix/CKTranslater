using CKTranslator.Contracts.Services;
using CKTranslator.Contracts.ViewModels;
using CKTranslator.Helpers;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace CKTranslator.Services
{
    public class NavigationService : INavigationService
    {
        private readonly IPageService pageService;
        private object lastParameterUsed;
        private Frame frame;
        private readonly ISettingsService settings;

        public event NavigatedEventHandler Navigated;

        public Frame Frame
        {
            get
            {
                if (frame == null)
                {
                    frame = Window.Current.Content as Frame;
                    RegisterFrameEvents();
                }

                return frame;
            }

            set
            {
                UnregisterFrameEvents();
                frame = value;
                RegisterFrameEvents();
            }
        }

        public bool CanGoBack => Frame.CanGoBack;

        public NavigationService(IPageService pageService, ISettingsService settings)
        {
            this.pageService = pageService;
            this.settings = settings;
        }

        private void RegisterFrameEvents()
        {
            if (frame != null)
            {
                frame.Navigated += OnNavigated;
            }
        }

        private void UnregisterFrameEvents()
        {
            if (frame != null)
            {
                frame.Navigated -= OnNavigated;
            }
        }

        public bool GoBack()
        {
            if (CanGoBack)
            {
                var vmBeforeNavigation = frame.GetPageViewModel();
                frame.GoBack();
                if (vmBeforeNavigation is INavigationAware navigationAware)
                {
                    navigationAware.OnNavigatedFrom();
                }
                return true;
            }

            return false;
        }

        public bool NavigateTo(string pageKey, object? parameter = null, bool clearNavigation = false)
        {
            var pageType = pageService.GetPageType(pageKey);

            if (frame.Content?.GetType() != pageType || (parameter != null && !parameter.Equals(lastParameterUsed)))
            {
                frame.Tag = clearNavigation;
                var vmBeforeNavigation = frame.GetPageViewModel();
                var navigated = frame.Navigate(pageType, parameter);
                if (navigated)
                {
                    lastParameterUsed = parameter;
                    settings.ActivePage = pageKey;

                    if (vmBeforeNavigation is INavigationAware navigationAware)
                    {
                        navigationAware.OnNavigatedFrom();
                    }
                }

                return navigated;
            }

            return false;
        }

        public void CleanNavigation()
            => frame.BackStack.Clear();

        private void OnNavigated(object sender, NavigationEventArgs e)
        {
            if (sender is Frame frame)
            {
                bool clearNavigation = (bool)frame.Tag;
                if (clearNavigation)
                {
                    frame.BackStack.Clear();
                }

                if (frame.GetPageViewModel() is INavigationAware navigationAware)
                {
                    navigationAware.OnNavigatedTo(e.Parameter);
                }

                Navigated?.Invoke(sender, e);
            }
        }
    }
}
