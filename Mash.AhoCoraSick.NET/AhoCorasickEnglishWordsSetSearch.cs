using Mash.HelperMethods.NET.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Mash.AhoCoraSick
{
    public class AhoCorasickEnglishWordsSetSearch : IAhoCorasickEnglishWordsSetSearch
    {

        private readonly object _lock = new object();
        internal const int AlphabetSize = 40;

        private readonly List<StateNode> _matchAsContainsAutomatonTree = new List<StateNode>() { new StateNode() };
        private readonly List<StateNodeWordMatch> _matchAsWordAutomatonTree = new List<StateNodeWordMatch>() { new StateNodeWordMatch() };
        //private int stateNoToStartFrom = 0;
        private bool _matchAsWord = false;
        private readonly bool _patternIsValidEnglishWord;

        /// Though SEOAnalyzer will always call with valid wordCharacter, we can remove the checks for further optimization,
        /// but kept for Other Third-party uses, as we can't guaranty if third-party provide valid wordCharter
        public bool IsAcceptablePattern(string s)
        {
            bool foundAlphabet = false;
            if (s.IsNullOrEmpty()) return false;
            for (int i = 0; i < s.Length; i++)
            {
                char ch = s[i];
                if (ch <= '9' && ch >= '&')
                {
                    if (ch == '.' || ch >= '0') continue;

                    if (ch == '-' || ch == '\'' || ch == '&')
                    {
                        if (i != 0 && i != s.Length - 1)
                        {
                            continue;
                        }

                        return false;
                    }
                }
                if (ch < 'A' || ch > 'z' || (ch > 'Z' && ch < 'a')) return false;
                foundAlphabet = true;

            }
            return (!this._patternIsValidEnglishWord || foundAlphabet);
        }
        private int AlphabetToIndexValue(char ch)
        {

            if (ch <= '9' && ch >= '&')
            {
                if (ch >= '0' && ch <= '9')
                {
                    return 30 + (ch - '0');
                }
                if (ch == '-')
                {
                    return 26;
                }

                if (ch == '\'')
                {
                    return 27;

                }

                if (ch == '&')
                {
                    return 28;
                }

                if (ch == '.')
                {
                    return 29;
                }
            }
            return ch.ToLowerValueIfUpperCase() - 'a';

        }
        public AhoCorasickEnglishWordsSetSearch(bool matchAsWord = false, bool patternIsValidEnglishWord = false)
        {
            this._matchAsWord = matchAsWord;
            this._patternIsValidEnglishWord = patternIsValidEnglishWord;
        }
        public AhoCorasickEnglishWordsSetSearch(string[] englishWordsSet, bool matchAsWord = false, bool patternIsValidEnglishWord = false)
        {
            this._matchAsWord = matchAsWord;
            this._patternIsValidEnglishWord = patternIsValidEnglishWord;
            if (englishWordsSet == null)
            {
                throw new ArgumentNullException(nameof(englishWordsSet));
            }

            foreach (string word in englishWordsSet)
            {
                this.AddEnglishWord(word);
            }

        }

        public void AddEnglishWord(string searchPattern)
        {
            lock (_lock)
            {

                if (searchPattern.IsNullOrEmpty())
                {
                    throw new ArgumentException("Search Word can't be null or empty");
                }

                if (!IsAcceptablePattern(searchPattern))
                {
                    throw new ArgumentException("Search Word can only contain English Alphabets and (' and - and & and . ) chars in the middle");
                }

                int currentNodeNo = 0;
                foreach (char ch in searchPattern.ToCharArray())
                {
                    int indexValue = this.AlphabetToIndexValue(ch);
                    if (_matchAsWord)
                    {
                        if (_matchAsWordAutomatonTree[currentNodeNo].NexNode[indexValue] == -1)
                        {
                            _matchAsWordAutomatonTree[currentNodeNo].NexNode[indexValue] =
                                _matchAsWordAutomatonTree.Count;
                            _matchAsWordAutomatonTree.Add(new StateNodeWordMatch());
                        }

                        currentNodeNo = _matchAsWordAutomatonTree[currentNodeNo].NexNode[indexValue];

                    }
                    else
                    {
                        if (_matchAsContainsAutomatonTree[currentNodeNo].NexNode[indexValue] == -1)
                        {
                            _matchAsContainsAutomatonTree[currentNodeNo].NexNode[indexValue] =
                                _matchAsContainsAutomatonTree.Count;
                            _matchAsContainsAutomatonTree.Add(new StateNode(currentNodeNo, (byte)ch));
                        }

                        currentNodeNo = _matchAsContainsAutomatonTree[currentNodeNo].NexNode[indexValue];
                    }

                }

                if (_matchAsWord)
                    _matchAsWordAutomatonTree[currentNodeNo].IsMatch = true;
                else
                    _matchAsContainsAutomatonTree[currentNodeNo].IsMatch = true;
            }
        }

        private int NextNodeToGoForPatternContainsMatch(int currentNodeNo, int charIndex)
        {

            StateNode currentNode = _matchAsContainsAutomatonTree[currentNodeNo];
            if (currentNode.NextNodeToGo[charIndex] == -1)
            {
                if (currentNode.NexNode[charIndex] != -1)
                {
                    currentNode.NextNodeToGo[charIndex] = currentNode.NexNode[charIndex];
                }
                else
                {
                    currentNode.NextNodeToGo[charIndex] =
                        currentNodeNo == 0 ? 0 : NextNodeToGoForPatternContainsMatch(GetFailureLink(currentNodeNo), charIndex);
                    currentNode.UsedFailureLink[charIndex] = true;
                }
            }

            return currentNode.NextNodeToGo[charIndex];

        }
        private int NextNodeToGoForMatchAsWord(int currentNodeNo, int charIndex)
        {

            StateNodeWordMatch currentNode = _matchAsWordAutomatonTree[currentNodeNo];
            return currentNode.NexNode[charIndex];

        }

        private int GetFailureLink(int currentNodeNo)
        {
            StateNode currentNode = _matchAsContainsAutomatonTree[currentNodeNo];
            if (currentNode.FailureLink == -1)
            {
                if (currentNodeNo == 0 || currentNode.ParentNodeNo == 0)
                {
                    currentNode.FailureLink = 0;
                }
                else
                {
                    currentNode.FailureLink =
                        NextNodeToGoForPatternContainsMatch(GetFailureLink(currentNode.ParentNodeNo), this.AlphabetToIndexValue((char)currentNode.ParentChar));
                }

            }

            return currentNode.FailureLink;
        }

        public bool GoToCharacter(char nextChar, int stateNoToStartFrom, out int newStateNo)
        {
            lock (_lock)
            {
                newStateNo = -1;
                int indexValue = this.AlphabetToIndexValue(nextChar);
                if (indexValue < 0 || indexValue >= AlphabetSize)
                {
                    return false;

                }

                if (_matchAsWord)
                {
                    newStateNo = NextNodeToGoForMatchAsWord(stateNoToStartFrom, indexValue);
                    if (newStateNo == -1)
                    {
                        return false;
                    }

                    return _matchAsWordAutomatonTree[newStateNo].IsMatch;
                }
                else
                {
                    newStateNo = NextNodeToGoForPatternContainsMatch(stateNoToStartFrom, indexValue);

                    return _matchAsContainsAutomatonTree[newStateNo].IsMatch;
                }
            }
        }

        //public void ResetSearchState()
        //{
        //    lock (_lock)
        //    {
        //        this._currentNodeNo = 0;
        //    }
        //}
    }
}
