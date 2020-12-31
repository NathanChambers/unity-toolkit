using System.Collections.Generic;
using UnityEngine;

namespace Toolkit {

    [CreateAssetMenu]
    public class StringTable : ScriptableObject {

        [System.Serializable]
        public struct Term {
            public string Key;
            public string Value;
        }

        public List<Term> _editorTerms = new List<Term>();

        [System.NonSerialized] private Dictionary<string, string> terms = new Dictionary<string, string>();

        public void Load() {
            foreach (var term in _editorTerms) {
                Requires.True(terms.ContainsKey(term.Key) == false);
                terms.Add(term.Key, term.Value);
            }
        }

        public bool Contains(string term) {
            return terms.ContainsKey(term);
        }

        public string Localise(string term) {
            Requires.True(terms.ContainsKey(term) == true);
            return terms[term];
        }

    }
}