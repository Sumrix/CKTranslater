﻿using System;
using System.Collections.Generic;

namespace Translation.Storages
{
    public class NotTranslatedDB : BaseDB<HashSet<string>>
    {
        public static NotTranslatedDB Load(string fileName = null)
        {
            return NotTranslatedDB.LoadFromFile<NotTranslatedDB>(fileName ?? FileName.NotTranslatedDB);
        }

        public bool Contains(string word)
        {
            return this.data.Contains(word);
        }

        public void Add(string word)
        {
            this.data.Add(word);
        }

        public void AddRange(IEnumerable<string> words)
        {
            this.data.UnionWith(words);
        }

        public void Clear()
        {
            this.data.Clear();
        }
    }
}
