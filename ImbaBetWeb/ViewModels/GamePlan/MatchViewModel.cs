using ImbaBetWeb.Models;
using ImbaBetWeb.ViewModels.DTO;

namespace ImbaBetWeb.ViewModels.GamePlan
{
    public class MatchViewModel
    {
        public Match Match {  get; set; }
        public IList<Bet> ActiveBets { get; set; }
        public IList<Bet> ClosedBets { get; set; }
    }
}
