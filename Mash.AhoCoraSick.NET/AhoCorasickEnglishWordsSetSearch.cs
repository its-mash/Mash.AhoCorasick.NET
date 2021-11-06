using Mash.HelperMethods.NET.ExtensionMethods;
using System;
using System.Collections.Generic;

namespace Mash.AhoCoraSick
{
    public class AhoCorasickEnglishWordsSetSearch : IAhoCorasickEnglishWordsSetSearch
    {

        private readonly object _lock = new object();

        private readonly List<StateNode> _automatonTree = new List<StateNode>() { new StateNode() };
        private int _currentNodeNo = 0;

        public AhoCorasickEnglishWordsSetSearch()
        {

        }
        public AhoCorasickEnglishWordsSetSearch(string[] englishWordsSet)
        {
            if (englishWordsSet == null)
            {
                throw new ArgumentNullException(nameof(englishWordsSet));
            }

            foreach (string word in englishWordsSet)
            {
                this.AddEnglishWord(word);
            }
        }

        public void AddEnglishWord(string searchWord)
        {
            lock (_lock)
            {

                if (searchWord.IsNullOrEmpty())
                {
                    throw new ArgumentException("Search Word can't be null or empty");
                }

                if (!searchWord.IsValidEnglishWord())
                {
                    throw new ArgumentException("Search Word can only contain English Alphabets");
                }

                int currentNode = 0;
                foreach (char ch in searchWord.ToCharArray())
                {
                    int charValue = ch.ToLowerValueIfUpperCase();
                    if (charValue == '-') continue;
                    charValue -= 'a';
                    if (_automatonTree[currentNode].NexNode[charValue] == -1)
                    {
                        _automatonTree[currentNode].NexNode[charValue] = _automatonTree.Count;
                        currentNode = _automatonTree.Count;
                    }

                }

                _automatonTree[currentNode].IsMatch = true;
            }
        }

        private int NextNodeToGo(int currentNodeNo, byte charValue)
        {
            StateNode currentNode = _automatonTree[currentNodeNo];
            if (currentNode.NextNodeToGo[charValue] == -1)
            {
                if (currentNode.NexNode[charValue] != -1)
                    currentNode.NextNodeToGo[charValue] = currentNode.NexNode[charValue];
                else
                {
                    currentNode.NextNodeToGo[charValue] =
                        currentNodeNo == 0 ? 0 : NextNodeToGo(GetFailureLink(currentNodeNo), charValue);
                }
            }

            return currentNode.NextNodeToGo[charValue];

        }

        private int GetFailureLink(int currentNodeNo)
        {
            StateNode currentNode = _automatonTree[currentNodeNo];
            if (currentNode.FailureLink == -1)
            {
                if (currentNodeNo == 0 || currentNode.ParentNodeNo == 0)
                {
                    currentNode.FailureLink = 0;
                }
                else
                {
                    currentNode.FailureLink =
                        NextNodeToGo(GetFailureLink(currentNode.ParentNodeNo), currentNode.ParentChar);
                }

            }

            return _automatonTree[currentNodeNo].FailureLink;
        }

        public bool GoToCharacter(char nextChar)
        {
            lock (_lock)
            {
                int charValue = nextChar.ToLowerValueIfUpperCase();
                if (charValue == '-')
                {
                    return false;
                }

                if (charValue < 'a' || charValue > 'z')
                {
                    this._currentNodeNo = 0;
                    return false;
                }

                _currentNodeNo = NextNodeToGo(_currentNodeNo, (byte)charValue);
                return _automatonTree[_currentNodeNo].IsMatch;
            }
        }

        public void ResetSearchState()
        {
            lock (_lock)
            {
                this._currentNodeNo = 0;
            }
        }
    }
}
