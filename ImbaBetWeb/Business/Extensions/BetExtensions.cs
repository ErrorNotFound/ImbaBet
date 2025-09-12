using ImbaBetWeb.Model;

namespace ImbaBetWeb.Business.Extensions
{
    public static class BetExtensions
    {
        public static bool IsActiveBet(this Bet bet)
        {
            return !bet.Match!.IsOver && DateTime.UtcNow <= bet.Match.DateTime;
        }

        public static bool IsClosedBet(this Bet bet)
        {
            return bet.Match!.IsOver;
        }

        /// <summary>
        /// Extracts the suggested winner from the bet.
        /// </summary>
        /// <returns>The team which the player bet to win. If he bet a draw, null is returned</returns>
        public static Team? GetSuggestedWinner(this Bet bet)
        {
            // if it's a draw, return null
            if (bet.GoalsA == bet.GoalsB)
            {
                return null;
            }

            return bet.GoalsA > bet.GoalsB ? bet.Match!.TeamA : bet.Match!.TeamB;
        }
    }
}
