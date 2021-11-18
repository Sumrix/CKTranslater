using CKTranslator.Core.Translation.Matching;

using System;

namespace SimilarityEditor
{
    public class WordsSimilarity
    {
        public readonly string Lang1Word;
        public readonly string? Lang2Word;
        public readonly WordMatch MatchInfo;

        public WordsSimilarity(string lang1Word, string? lang2Word, WordMatch matchInfo)
        {
            this.Lang1Word = lang1Word ?? throw new ArgumentNullException(nameof(lang1Word));
            this.Lang2Word = lang2Word ?? throw new ArgumentNullException(nameof(lang2Word));
            this.MatchInfo = matchInfo ?? throw new ArgumentNullException(nameof(matchInfo));
        }
    }
}