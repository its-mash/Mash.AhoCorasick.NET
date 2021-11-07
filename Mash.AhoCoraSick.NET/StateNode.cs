using System;
using System.Collections.Generic;
using System.Text;
using Mash.HelperMethods.NET.ExtensionMethods;

namespace Mash.AhoCoraSick
{
    internal class UsedFailureLink
    {
        private long _usedFailureLink;
        public UsedFailureLink()
        {
            _usedFailureLink = 0;

        }

        public int Length => AhoCorasickEnglishWordsSetSearch.AlphabetSize;
        public bool this[int index]
        {
            get
            {
                if (index >= AhoCorasickEnglishWordsSetSearch.AlphabetSize)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }
                return _usedFailureLink.GetBit(index);
            }
            set => _usedFailureLink.SetBit(index, value);
        }
    }
    internal class StateNode
    {
        public int[] NexNode;
        public bool IsMatch;
        public int ParentNodeNo;
        public byte ParentChar;
        public int FailureLink;
        public int[] NextNodeToGo;
        public UsedFailureLink UsedFailureLink;


        public StateNode(int parentNodeNo = -1, byte parentChar = 0, bool isMatch = false)
        {
            UsedFailureLink = new UsedFailureLink();
            this.ParentNodeNo = parentNodeNo;
            this.ParentChar = parentChar;
            this.NexNode = new int[AhoCorasickEnglishWordsSetSearch.AlphabetSize];
            this.NexNode.Fill(-1);
            this.IsMatch = isMatch;
            this.FailureLink = -1;
            this.NextNodeToGo = new int[AhoCorasickEnglishWordsSetSearch.AlphabetSize];
            this.NextNodeToGo.Fill(-1);
        }
    };
}
