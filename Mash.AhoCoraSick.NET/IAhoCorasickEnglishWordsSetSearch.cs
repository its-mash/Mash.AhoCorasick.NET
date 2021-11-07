namespace Mash.AhoCoraSick
{
    interface IAhoCorasickEnglishWordsSetSearch
    {
        void AddEnglishWord(string searchWord);

        bool GoToCharacter(char nextChar, out int newStateNo);

        void ResetSearchState();

    }
}
