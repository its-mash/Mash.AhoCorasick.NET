using System;
using System.Collections.Generic;
using System.Text;

namespace Mash.AhoCoraSick
{
    internal class StateNode
    {
        private const int AlphabetSize = 26;
        public int[] NexNode;
        public bool IsMatch;
        public int ParentNodeNo;
        public byte ParentChar;
        public int FailureLink;
        public int[] NextNodeToGo;

        public StateNode(int parentNodeNo = -1, byte parentChar = 0, bool isMatch = false)
        {

            this.ParentNodeNo = parentNodeNo;
            this.ParentChar = parentChar;
            this.NexNode = new int[AlphabetSize];
            Array.Fill(this.NexNode, -1);
            this.IsMatch = isMatch;
            this.FailureLink = -1;
            this.NextNodeToGo = new int[AlphabetSize];
            Array.Fill(this.NextNodeToGo, -1);
        }
    };
}
