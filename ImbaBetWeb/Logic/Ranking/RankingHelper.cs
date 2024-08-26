
using ImbaBetWeb.Logic;
using ImbaBetWeb.Logic.Ranking.Details;
using ImbaBetWeb.Models;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json.Linq;

namespace ImbaBetWeb.Logic.Ranking
{
    public static class RankingHelper
    {
        public static void SortAndSetRanks<T>(List<RankingItem<T>> list, IComparer<RankingItem<T>> comparer) where T : class
        {
            list.Sort(comparer);

            var currentRank = 1;

            var first = list.FirstOrDefault();
            if(first != null)
            {
                first.Rank = currentRank;
            }
            
            for (var i = 1; i < list.Count; i++)
            {
                var dCompare = comparer.Compare(list[i - 1], list[i]);
                if (dCompare < 0)
                {
                    list[i].Rank = i + 1;
                    currentRank = i + 1;
                }
                else if (dCompare == 0)
                {
                    list[i].Rank = currentRank;
                }
            }
        }
    }
}
