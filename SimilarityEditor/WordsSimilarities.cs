using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using BrightIdeasSoftware;
using Core;
using Core.Matching;
using Core.Storages;
using Core.Transliteration;

namespace SimilarityEditor
{
    public class WordsSimilarities : IVirtualListDataSource
    {
        private readonly Language language0 = Language.Load(DB.EngLetters);
        private readonly Language language1 = Language.Load(DB.RusLetters);
        private readonly string[] toTranslateWords = File.ReadAllLines(FileName.ToTranslateWords);
        private string filter;
        private List<WordsSimilarity> filteredWords;
        private List<WordsSimilarity> words;

        public WordsSimilarities()
        {
            this.words = new List<WordsSimilarity>();
        }

        public string Filter
        {
            get => this.filter;
            set
            {
                if (this.filter != value)
                {
                    this.filter = value;
                    this.DoFilter();
                }
            }
        }

        public void AddObjects(ICollection modelObjects)
        {
            throw new NotImplementedException();
        }

        public object GetNthObject(int n)
        {
            return this.filteredWords[n];
        }

        public int GetObjectCount()
        {
            return this.filteredWords.Count;
        }

        public int GetObjectIndex(object model)
        {
            return this.filteredWords.IndexOf((WordsSimilarity) model);
        }

        public void InsertObjects(int index, ICollection modelObjects)
        {
            throw new NotImplementedException();
        }

        public void PrepareCache(int first, int last)
        {
        }

        public void RemoveObjects(ICollection modelObjects)
        {
            throw new NotImplementedException();
        }

        public int SearchText(string value, int first, int last, OLVColumn column)
        {
            throw new NotImplementedException();
        }

        public void SetObjects(IEnumerable collection)
        {
            throw new NotImplementedException();
        }

        public void Sort(OLVColumn column, SortOrder order)
        {
            throw new NotImplementedException();
        }

        public void UpdateObject(int index, object modelObject)
        {
            throw new NotImplementedException();
        }

        private void DoFilter()
        {
            if (string.IsNullOrEmpty(this.filter))
            {
                this.filteredWords = this.words;
            }
            else
            {
                this.filteredWords = this.words
                    .Where(ws => ws.Lang1Word.EqualsWildcard(this.filter) ||
                                 ws.Lang2Word.EqualsWildcard(this.filter))
                    .ToList();
            }
        }

        public void Rebuild()
        {
            this.RecalcSimilarities();
            this.DoFilter();
        }

        private void RecalcSimilarities()
        {
            var wordsToLearn = DB.Translations
                .Union(DB.EngToRusMap.Select(x => new WordInLangs(x.eng.ToString(), x.rus)));

            this.words = wordsToLearn
                .Select(w => new WordsSimilarity(
                    w.Lang1Word,
                    w.Lang2Word,
                    WordMatch.Create(w.Lang1Word, w.Lang2Word, this.language0, this.language1)
                ))
                .OrderBy(x => x.MatchInfo.Similarity)
                .ToList();
        }
    }
}