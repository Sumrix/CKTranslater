using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Core.Parsing;
using Path = Core.Parsing.Path;

namespace Core.Processing
{
    /// <summary>
    ///     Временный класс для анализа скриптов игры и выявления строковых идентификаторов.
    ///     В дальнейшем эти данные используются для создания фильтра, по которому будут работать парсеры скриптов.
    /// </summary>
    public class StringAnalyzer
    {
        private readonly LocalisationLoader engLocals;
        private readonly ScriptValuesLoader engScripts;
        private readonly LocalisationLoader rusLocals;
        private readonly ScriptValuesLoader rusScripts;

        public StringAnalyzer()
        {
            this.rusScripts = new ScriptValuesLoader { Language = Language.Eng };
            this.rusLocals = new LocalisationLoader { Language = Language.Eng };
            this.engScripts = new ScriptValuesLoader { Language = Language.Eng };
            this.engLocals = new LocalisationLoader { Language = Language.Eng };
        }

        public void FilterAndSaveResults()
        {
            IdManager.IgnoreValues.UnionWith(
                this.engScripts.Strings
                    .Union(this.rusScripts.Strings)
                    .Where(x => x.Key.Path.FirstStep == @"common\game_rules" &&
                                x.Key.Path.LastTwoSteps == "option.name" ||
                                x.Key.Path.ToString() == @"localisation\customizable_localisation.defined_text.name")
                    .Select(x => x.Value));

            //List<string> strs = this.rusScripts.Strings
            //        .Where(x => !IdManager.Identifiers.Contains(x.Value) &&
            //                    !x.Value.Any(IsRusLetter) &&
            //                    x.Value.Length > 0 &&
            //                    x.Key.Path.LastStep != "log" &&
            //                    !IdManager.IgnoreKeys.Any(pattern => x.Key.Path.LastStep.EqualsWildcard(pattern)) &&
            //                    !char.IsUpper(x.Value.First()) &&
            //                    !(x.Value.Length > 3 && x.Value.EqualsWildcard("a?-*")) &&
            //                    x.Key.Path.LastStep != "from_dynasty_prefix" &&
            //                    x.Key.Path.LastStep != "female_patronym" &&
            //                    x.Key.Path.LastStep != "male_patronym" &&
            //                    x.Key.Path.LastStep != "from_dynasty_suffix")
            //        .Select(x => x.ToString())
            //        .ToList();

            //SaveToFile(strs, "strs.txt");

            //var ids = IdManager.Identifiers.OrderBy(x => x);

            //SaveToFile(ids, "ids.txt");

            var name2 = new HashSet<string>
            {
                "add_character_modifier.name",
                "character_event.name",
                "create_character.name",
                "defined_text.name",
                "has_game_rule.name",
                "option.name",
                "per_attribute.name",
                "add_province_modifier.name"
            };

            var strs = this.rusScripts.Strings
                .Union(this.engScripts.Strings)
                .Where(x => !IdManager.IgnoreValueRegex.IsMatch(x.Value) &&
                            IdManager.StringKeys.Any(keyPattern => x.Key.Path.LastStep.EqualsWildcard(keyPattern)) &&
                            x.Value.Length > 0 &&
                            //!x.Value.StartsWith("Get") &&
                            //x.Key.Path.LastStep != "log" &&
                            //!IdManager.IgnoreKeys.Any(pattern => x.Key.Path.LastStep.ToLower().EqualsWildcard(pattern))&&
                            !x.Value.Any(StringAnalyzer.IsRusLetter) &&
                            !name2.Contains(x.Key.Path.LastTwoSteps) &&
                            !(x.Value.Length > 1 && char.IsUpper(x.Value.First()) &&
                              x.Value.Skip(1).All(c => char.IsLower(c))));

            // Идентификаторы которым присваиваются строковые значения
            var stringIdentifiers = strs
                .Select(x => x.Key.Path.LastStep)
                .ToHashSet();

            // Пути по которым идентификаторам присваиваются строковые значения
            var stringPaths = strs
                .GroupBy(x => string.Join(".", x.Key.Path.Steps.Reverse<string>()), x => x.Value)
                .Select(x => x.Key + " = " + x.First())
                //.Select(x => string.Join(".", x.Key.Path.Steps.Reverse<string>()) + " = " + x.Value)
                //.Select(x => x.Value + " = " + string.Join(".", x.Key.Path.Steps.Reverse<string>()))
                .ToHashSet();

            StringAnalyzer.SaveToFile("StringIdentifiers.txt", stringIdentifiers);
            StringAnalyzer.SaveToFile("StringPaths.txt", stringPaths);


            //HashSet<string> rusLastSteps = rusLocals.Strings
            //    .Select(x => x.Key.Path.LastStep)
            //    .ToHashSet();
            //HashSet<string> engLastSteps = engLocals.Strings
            //    .Select(x => x.Key.Path.LastStep)
            //    .ToHashSet();

            //HashSet<Parsing.Path> strPaths = rusScripts.Strings
            //    .Where(s => s.Value.Any(IsRusLetter) ||
            //                s.Value.Contains(' ') ||
            //                rusLastSteps.Contains(s.Value) ||
            //                engLastSteps.Contains(s.Value))
            //    .Select(s => s.Key.Path)
            //    .ToHashSet()
            //    .Union(
            //        engScripts.Strings
            //        .Where(s => rusLastSteps.Contains(s.Value) ||
            //                    engLastSteps.Contains(s.Value))
            //        .Select(s => s.Key.Path)
            //        .ToHashSet()
            //    )
            //    .ToHashSet();
            //HashSet<Parsing.Path> idPaths = rusScripts.Strings
            //    .Where(s => !IdManager.Identifiers.Contains(s.Value) &&
            //                IdManager.Regex.IsMatch(s.Value))
            //    .Select(s => s.Key.Path)
            //    .ToHashSet()
            //    .Union(
            //        engScripts.Strings
            //            .Where(s => !IdManager.Identifiers.Contains(s.Value) &&
            //                        IdManager.Regex.IsMatch(s.Value))
            //            .Select(s => s.Key.Path)
            //            .ToHashSet()
            //    )
            //    .Except(strPaths)
            //    .ToHashSet();
            //HashSet<Parsing.Path> otherPaths = rusScripts.Strings
            //    .Select(s => s.Key.Path)
            //    .ToHashSet()
            //    .Union(engScripts.Strings.Select(s => s.Key.Path).ToHashSet())
            //    .Except(strPaths)
            //    .Except(idPaths)
            //    .ToHashSet();

            //HashSet<string> otherLastSteps = otherPaths.Select(x => x.LastStep).ToHashSet();
            //HashSet<string> idLastSteps = idPaths.Select(x => x.LastStep).ToHashSet();
            //HashSet<string> strLastSteps = strPaths.Select(x => x.LastStep).ToHashSet();
            //HashSet<string> strs1 = strLastSteps
            //    .Except(idLastSteps)
            //    .Except(otherLastSteps)
            //    .ToHashSet();
            //HashSet<string> ids1 = idLastSteps
            //    .Except(strLastSteps)
            //    .Except(otherLastSteps)
            //    .ToHashSet();
            //HashSet<string> others1 = otherLastSteps
            //    .Except(strLastSteps)
            //    .Except(idLastSteps)
            //    .ToHashSet();

            //HashSet<string> otheLastTwoSteps = otherPaths
            //    .Where(x => !others1.Contains(x.LastStep))
            //    .Select(x => x.LastTwoSteps)
            //    .ToHashSet();
            //HashSet<string> idLastTwoSteps = idPaths
            //    .Where(x => !ids1.Contains(x.LastStep))
            //    .Select(x => x.LastTwoSteps)
            //    .ToHashSet();
            //HashSet<string> strLastTwoSteps = strPaths
            //    .Where(x => !strs1.Contains(x.LastStep))
            //    .Select(x => x.LastTwoSteps)
            //    .ToHashSet();
            //HashSet<string> strs2 = strLastTwoSteps
            //    .Except(idLastTwoSteps)
            //    .Except(otheLastTwoSteps)
            //    .ToHashSet();
            //HashSet<string> ids2 = idLastTwoSteps
            //    .Except(strLastTwoSteps)
            //    .Except(otheLastTwoSteps)
            //    .ToHashSet();
            //HashSet<string> others2 = otheLastTwoSteps
            //    .Except(strLastTwoSteps)
            //    .Except(idLastTwoSteps)
            //    .ToHashSet();

            //HashSet<string> strs3 = strPaths
            //    .Where(x => !strs1.Contains(x.LastStep))
            //    .Where(x => !strs2.Contains(x.LastTwoSteps))
            //    .Select(x => string.Join(".", x.Steps.Reverse<string>()))
            //    .ToHashSet();
            //HashSet<string> ids3 = idPaths
            //    .Where(x => !ids1.Contains(x.LastStep))
            //    .Where(x => !ids2.Contains(x.LastTwoSteps))
            //    .Select(x => string.Join(".", x.Steps.Reverse<string>()))
            //    .ToHashSet();
            //HashSet<string> others3 = otherPaths
            //    .Where(x => !others1.Contains(x.LastStep))
            //    .Where(x => !others2.Contains(x.LastTwoSteps))
            //    .Select(x => string.Join(".", x.Steps.Reverse<string>()))
            //    .ToHashSet();

            //var strsTotal = strs1
            //    .Union(strs2)
            //    .Union(strs3);
            //var idsTotal = ids1
            //    .Union(ids2)
            //    .Union(ids3);
            //var othersTotal = others1
            //    .Union(others2)
            //    .Union(others3);

            //SaveToFile(strPaths.Select(x => string.Join(".", x.Steps.Reverse<string>())), "strPaths.txt");
            //SaveToFile(idPaths.Select(x => string.Join(".", x.Steps.Reverse<string>())), "idPaths.txt");
            //SaveToFile(otherPaths.Select(x => string.Join(".", x.Steps.Reverse<string>())), "otherPaths.txt");
            //SaveToFile(strsTotal, "strs.txt");
            //SaveToFile(idsTotal, "ids.txt");
            //SaveToFile(othersTotal, "others.txt");
        }

        private static bool IsRusLetter(char letter)
        {
            return 'а' <= letter && letter <= 'я' || 'А' <= letter && letter <= 'Я';
        }

        public void Load()
        {
            this.LoadStrings("RusStrings.txt", this.rusScripts.Strings);
            this.LoadStrings("RusLocals.txt", this.rusLocals.Strings);
            this.LoadStrings("EngStrings.txt", this.engScripts.Strings);
            this.LoadStrings("EngLocals.txt", this.engLocals.Strings);

            IdManager.IgnoreValues =
                File.ReadAllLines(@"..\..\..\Data\Strings\Identifiers.txt")
                    .ToHashSet();
            IdManager.Cultures =
                File.ReadAllLines(@"..\..\..\Data\Strings\Cultures.txt")
                    .ToHashSet();
        }

        public Event LoadEngFiles(FileContext context)
        {
            return context.ModFolder == "localisation"
                ? this.engLocals.Load(context)
                : this.engScripts.Load(context);
        }

        public Event LoadRusFiles(FileContext context)
        {
            return context.ModFolder == "localisation"
                ? this.rusLocals.Load(context)
                : this.rusScripts.Load(context);
        }

        private void LoadStrings(string fileName, Dictionary<ScriptKey, string> strings)
        {
            Regex regex = new(@"^\[(\d+)\](.+?) = (.+)$");

            strings.Clear();

            foreach (string line in File.ReadAllLines(@"..\..\..\Data\Strings\" + fileName))
            {
                Match match = regex.Match(line);
                if (!match.Success)
                {
                    throw new Exception();
                }

                int repetitionIndex = int.Parse(match.Groups[1].Value);
                var steps = match.Groups[2].Value.Split('.').ToList();
                string value = match.Groups[3].Value;

                strings.Add(
                    new ScriptKey
                    {
                        Path = new Path(steps),
                        RepetitionIndex = repetitionIndex
                    },
                    value);
            }
        }

        public void Save()
        {
            this.SaveStrings("RusStrings.txt", this.rusScripts.Strings);
            this.SaveStrings("RusLocals.txt", this.rusLocals.Strings);
            this.SaveStrings("EngStrings.txt", this.engScripts.Strings);
            this.SaveStrings("EngLocals.txt", this.engLocals.Strings);

            File.WriteAllLines(@"..\..\..\Data\Strings\Identifiers.txt",
                IdManager.IgnoreValues.OrderBy(x => x)
            );
            File.WriteAllLines(@"..\..\..\Data\Strings\Cultures.txt",
                IdManager.Cultures.OrderBy(x => x)
            );
        }

        private void SaveStrings(string fileName, Dictionary<ScriptKey, string> strings)
        {
            StringAnalyzer.SaveToFile(
                @"..\..\..\Data\Strings\" + fileName,
                strings
                    .Where(x => !string.IsNullOrEmpty(x.Value))
                    .Select(x => string.Format("{0} = {1}", x.Key, x.Value))
                    .OrderBy(x => x)
            );
        }

        private static void SaveToFile(string fileName, IEnumerable<string> strs)
        {
            string path = System.IO.Path.Combine(@"..\..\..\Data\Strings\", fileName);
            File.WriteAllText(path, string.Join("\n", strs.OrderBy(x => x)), Encoding.GetEncoding("Windows-1251"));
        }
    }
}