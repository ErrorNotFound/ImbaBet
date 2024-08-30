using ImbaBetWeb.Models;
using ImbaBetWeb.ViewModels.DTO;

namespace ImbaBetWeb.ViewModels.GamePlan
{
    public class MatchViewModel
    {
        public required Match Match {  get; set; }
        public required IList<Bet> ActiveBets { get; set; }
        public required IList<Bet> ClosedBets { get; set; }
    }
}
