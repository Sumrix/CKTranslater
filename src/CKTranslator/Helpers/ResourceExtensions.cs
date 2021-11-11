using Microsoft.Windows.ApplicationModel.Resources;

namespace CKTranslator.Helpers
{
    internal static class ResourceExtensions
    {
        private static readonly ResourceManager resourceManager = new();

        public static string GetLocalized(this string resourceKey)
        {
            var resourceCandidate = resourceManager.MainResourceMap.GetValue("Resources/" + resourceKey);
            return resourceCandidate.ValueAsString;
        }
    }
}
