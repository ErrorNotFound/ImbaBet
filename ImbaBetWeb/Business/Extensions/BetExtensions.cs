using ImbaBetWeb.Model;

namespace ImbaBetWeb.Business.Extensions
{
    public static class BetExtensions
    {
        public static bool IsDrawBet(this Bet bet)
        {
            return bet.GoalsA == bet.GoalsB;
        }

        public static bool IsActiveBet(this Bet bet)
        {
            return !bet.Match!.IsOver && DateTime.UtcNow >= bet.Match.DateTime;
        }

        public static bool IsClosedBet(this Bet bet)
        {
            return bet.Match!.IsOver;
        }

        public static Team? GetSuggestedWinner(this Bet bet)
        {
            if (bet.IsDrawBet())
            {
                return null;
            }

            if(bet.Match == null)
            {
                throw new InvalidOperationException("Match property is not loaded.");
            }

            if(bet.Match.TeamA == null || bet.Match.TeamB == null)
            {
                throw new InvalidOperationException("Match teams are not loaded.");
            }

            return bet.GoalsA > bet.GoalsB ? bet.Match.TeamA : bet.Match.TeamB;
        }
    }
}
