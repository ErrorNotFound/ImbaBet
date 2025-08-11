using ImbaBetWeb.Model;

namespace ImbaBetWeb.ViewModels.Betting
{
    public class MyBetsViewModel
    {
        public required IEnumerable<Bet> OpenBets { get; set; }
        public required IEnumerable<Bet> ClosedBets { get; set; }
        public required IEnumerable<Bet> ActiveBets { get; set; }
    }
}
