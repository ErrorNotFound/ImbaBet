using ImbaBetWeb.Models;

namespace ImbaBetWeb.ViewModels.Betting
{
    public class MyBetsViewModel
    {
        public IList<Bet> OpenBets { get; set; }
        public IList<Bet> ClosedBets { get; set; }
        public IList<Bet> ActiveBets { get; set; }
    }
}
