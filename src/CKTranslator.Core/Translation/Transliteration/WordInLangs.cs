﻿using Newtonsoft.Json;

namespace CKTranslator.Core.Translation.Transliteration
{
    public class WordInLangs
    {
        public readonly string Lang1Word;
        public readonly string? Lang2Word;

        public WordInLangs(string source, string? target)
        {
            this.Lang1Word = source;
            this.Lang2Word = target;
        }

        [JsonIgnore] public bool IsTranslated => !string.IsNullOrEmpty(this.Lang2Word);

        public override string ToString()
        {
            return $"{this.Lang1Word} = {this.Lang2Word}";
        }
    }
}