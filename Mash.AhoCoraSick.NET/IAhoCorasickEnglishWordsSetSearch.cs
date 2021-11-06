namespace Mash.AhoCoraSick
{
    interface IAhoCorasickEnglishWordsSetSearch
    {
        void AddEnglishWord(string searchWord);

        bool GoToCharacter(char nextChar);

        void ResetSearchState();

    }
}
