using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Core.Parsing
{
    public static class IdManager
    {
        public static HashSet<string> Cultures;
        public static readonly Regex IgnoreValueRegex;
        public static HashSet<string> IgnoreValues;
        public static LinkedList<string> NotTransleated = new LinkedList<string>();
        public static string[] StringKeys;
        public static LinkedList<string> Transleated = new LinkedList<string>();

        static IdManager()
        {
            IdManager.IgnoreValues = IdManager.LoadValues(FileName.IgnoreValues);
            IdManager.StringKeys = IdManager.LoadValues(FileName.StringKeys).ToArray();

            IdManager.IgnoreValueRegex = new Regex(@"^([A-Z0-9]{2,}|[a-zA-Z0-9@:_\.-]*[_\.][a-zA-Z0-9@:_\.-]*)$");

            IdManager.Cultures = new HashSet<string>();
        }

        public static void AddValueToIgnore(string value)
        {
            if (!IdManager.IgnoreValueRegex.IsMatch(value))
            {
                IdManager.IgnoreValues.Add(value);
            }
        }

        public static string EncodeCulture(string cultureName)
        {
            return "CULTURE_" + cultureName;
        }

        private static HashSet<string> LoadValues(string fileName)
        {
            return File.ReadAllLines(fileName)
                .Select(x => x.Trim())
                .Where(x => x.Length > 0 &&
                            !x.StartsWith("#"))
                .ToHashSet();
        }
    }
}