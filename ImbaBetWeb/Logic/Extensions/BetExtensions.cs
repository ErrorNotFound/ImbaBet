using ImbaBetWeb.Models;

namespace ImbaBetWeb.Logic.Extensions
{
    public static class BetExtensions
    {
        public static bool IsDrawBet(this Bet bet)
        {
            return bet.GoalsA == bet.GoalsB;
        }

        public static Team? GetSuggestedWinner(this Bet bet)
        {
            if (bet.IsDrawBet())
            {
                return null;
            }

            return bet.GoalsA > bet.GoalsB ? bet.Match.TeamA : bet.Match.TeamB;
        }
    }
}
