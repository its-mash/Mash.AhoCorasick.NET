namespace Mash.AhoCoraSick
{
    interface IAhoCorasickEnglishWordsSetSearch
    {
        void AddEnglishWord(string searchPattern);

        bool GoToCharacter(char nextChar, int stateNoToStartFrom, out int newStateNo);

        //void ResetSearchState();

    }
}
