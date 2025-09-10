using ImbaBetWeb.Model;
using ImbaBetWeb.ViewModels.DTO;

namespace ImbaBetWeb.ViewModels.GamePlan
{
    public class MatchViewModel
    {
        public required Match Match {  get; set; }
        public required IEnumerable<Bet> ActiveBets { get; set; }
        public required IEnumerable<Bet> ClosedBets { get; set; }
    }
}
