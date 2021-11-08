namespace Mash.AhoCoraSick
{
    interface IAhoCorasickEnglishWordsSetSearch
    {
        void AddEnglishWord(string searchWord);

        bool GoToCharacter(char nextChar, int stateNoToStartFrom, out int newStateNo);

        //void ResetSearchState();

    }
}
