﻿using System.Text.Json.Serialization;

namespace VocabTrack.Core
{
    public class VocabularyModel
    {
        [JsonPropertyName("learned")]
        public NodeModel Learned { get; set; } = new NodeModel();

        [JsonPropertyName("names")]
        public NodeModel Names { get; set; } = new NodeModel();

        [JsonPropertyName("todo")]
        public NodeModel Todo { get; set; } = new NodeModel();

        public bool ContainsWord(string word)
        {
            return Learned.ContainsWord(word) || Names.ContainsWord(word) || Todo.ContainsWord(word);
        }

        public class NodeModel : Dictionary<string, NodeModel>
        {
            public bool ContainsWord(string word)
            {
                var symbols = word.ToLowerInvariant().ToList().Select(s => s.ToString()).ToList();
                return ContainsWord(symbols, 0);
            }

            public void Add(string word)
            {
                var symbols = word.ToLowerInvariant().ToList().Select(s => s.ToString()).ToList();
                Add(symbols, 0);
            }

            private bool ContainsWord(List<string> symbols, int index)
            {
                if (symbols.Count == index)
                    return true;

                var isLast = index == symbols.Count - 1;
                var lower = symbols[index];
                var upper = lower.ToUpperInvariant();

                if (!TryGetValue(upper, out var child))
                {
                    if (!TryGetValue(lower, out child))
                    {
                        return false;
                    }
                    else
                    {
                        if (isLast)
                            return false;
                    }
                }

                return child.ContainsWord(symbols, index + 1);
            }

            private void Add(List<string> symbols, int index)
            {
                if (symbols.Count == index)
                    return;

                var isLast = index == symbols.Count - 1;
                var lower = symbols[index];
                var upper = lower.ToUpperInvariant();

                if (!TryGetValue(upper, out var child))
                {
                    if (!TryGetValue(lower, out child))
                    {
                        child = new NodeModel();
                        Add(isLast ? upper : lower, child);
                    }
                    else
                    {
                        if (isLast)
                        {
                            Remove(lower);
                            Add(upper, child);
                        }
                    }
                }

                child.Add(symbols, index + 1);
            }
        }
    }
}
