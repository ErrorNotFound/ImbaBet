using ImbaBetWeb.Models;

namespace ImbaBetWeb.ViewModels.Betting
{
    public class MyBetsViewModel
    {
        public required IList<Bet> OpenBets { get; set; }
        public required IList<Bet> ClosedBets { get; set; }
        public required IList<Bet> ActiveBets { get; set; }
    }
}
