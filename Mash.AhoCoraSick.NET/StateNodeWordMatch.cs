using System;
using System.Collections.Generic;
using System.Text;
using Mash.HelperMethods.NET.ExtensionMethods;

namespace Mash.AhoCoraSick
{
    class StateNodeWordMatch
    {
        public int[] NexNode;
        public bool IsMatch;


        public StateNodeWordMatch(bool isMatch = false)
        {
            this.NexNode = new int[AhoCorasickEnglishWordsSetSearch.AlphabetSize];
            this.NexNode.Fill(-1);
            this.IsMatch = isMatch;
        }
    }
}
