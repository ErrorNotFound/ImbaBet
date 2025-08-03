namespace ImbaBetWeb.Logic.Ranking
{
    public static class RankingHelper
    {
        /// <summary>
        /// Sorts the given list with the given comparer. Then the rank of each item is set, starting with 1. If two or more items are equal, they are given the same rank.
        /// </summary>
        public static void SortDescendingAndSetRanks<T>(List<RankingItem<T>> list, IComparer<RankingItem<T>> comparer) where T : class
        {
            list.Sort(comparer);
            list.Reverse();

            for(int i = 0, currentRank = 1; i < list.Count; i++)
            {
                if(i+1 == list.Count)
                {
                    list[i].Rank = currentRank;
                    break;
                }

                var compareResult = comparer.Compare(list[i], list[i + 1]);
                if(compareResult > 0) // [i] > [i+1]
                {
                    list[i].Rank = currentRank;
                    currentRank++;
                }
                else if(compareResult == 0) // [i] == [i+1]
                {
                    list[i].Rank = currentRank;
                }
                else // [i] < [i+1]
                {
                    // means the given list is not sorted as expected
                    throw new Exception("The given list was not sorted correctly: " + string.Join(",", list.Select(x => x.Details.ToString())));
                }
            }
        }
    }
}
