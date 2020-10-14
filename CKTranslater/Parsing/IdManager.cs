using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace CKTranslater.Parsing
{
    public static class IdManager
    {
        public static HashSet<string> IgnoreValues;
        public static readonly Regex IgnoreValueRegex;
        public static HashSet<string> Cultures;
        public static string[] StringKeys;
        public static LinkedList<string> Transleated = new LinkedList<string>();
        public static LinkedList<string> NotTransleated = new LinkedList<string>();

        static IdManager()
        {
            IgnoreValues = LoadValues(FileName.IgnoreValues);
            StringKeys = LoadValues(FileName.StringKeys).ToArray();

            IgnoreValueRegex = new Regex(@"^([A-Z0-9]{2,}|[a-zA-Z0-9@:_\.-]*[_\.][a-zA-Z0-9@:_\.-]*)$");

            Cultures = new HashSet<string>();
        }

        public static void AddValueToIgnore(string value)
        {
            if (!IgnoreValueRegex.IsMatch(value))
            {
                IgnoreValues.Add(value);
            }
        }

        private static HashSet<string> LoadValues(string fileName)
        {
            return File.ReadAllLines(fileName)
                .Select(x => x.Trim())
                .Where(x => x.Length > 0 &&
                            !x.StartsWith("#"))
                .ToHashSet();
        }

        public static string EncodeCulture(string cultureName)
        {
            return "CULTURE_" + cultureName;
        }
    }
}
