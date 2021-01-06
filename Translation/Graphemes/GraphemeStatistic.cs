using System.Collections.Generic;
using System.Linq;

namespace NameTranslation.Graphemes
{
    public class GraphemeStatistic
    {
        public readonly Dictionary<string, Dictionary<string, int[]>> Values;

        private GraphemeStatistic(Dictionary<string, Dictionary<string, int[]>> values)
        {
            this.Values = values;
        }

        public static GraphemeStatistic Create(IEnumerable<GraphemeTranslation> graphemeTranslations)
        {
            var statistic = new Dictionary<string, Dictionary<string, int[]>>();

            var z = graphemeTranslations
                .Where(x => x.Original.Flags.Value == 2)
                .ToList();

            foreach (GraphemeTranslation match in graphemeTranslations)
            {
                if (statistic.TryGetValue(match.Original.Letters, out var translationStatistic))
                {
                    if (translationStatistic.TryGetValue(match.Translation, out int[]? flagsStatistic))
                    {
                        flagsStatistic[match.Original.Flags.Value]++;
                    }
                    else
                    {
                        flagsStatistic = new int[Grapheme.FlagVariants[(int) match.Original.Type]];
                        flagsStatistic[match.Original.Flags.Value] = 1;
                        translationStatistic[match.Translation] = flagsStatistic;
                    }
                }
                else
                {
                    int[] flagsStatistic = new int[Grapheme.FlagVariants[(int) match.Original.Type]];
                    flagsStatistic[match.Original.Flags.Value] = 1;

                    statistic[match.Original.Letters] = new Dictionary<string, int[]>
                    {
                        { match.Translation, flagsStatistic }
                    };
                }
            }

            return new GraphemeStatistic(statistic);
        }

        public override string ToString()
        {
            return
                string.Join(
                    "\n",
                    this.Values
                        .OrderBy(g => g.Key)
                        .Select(g => string.Format("{0}\n├─{1}", g.Key,
                            string.Join("\n├─", g.Value.Select(t =>
                                string.Format("{0}\n│ ├─{1}", t.Key,
                                    string.Join("\n│ ├─", t.Value.Select((f, i) =>
                                            f == 0 ? null : string.Format("{0} - {1}", Bit.ToString((uint) i, 6), f))
                                        .Where(s => s != null))))))
                        )
                );
        }
    }
}