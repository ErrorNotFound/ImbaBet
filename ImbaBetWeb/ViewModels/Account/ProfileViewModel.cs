using ImbaBetWeb.Model;
using ImbaBetWeb.ViewModels.DTO;

namespace ImbaBetWeb.ViewModels.Account
{
	public class ProfileViewModel
	{
		public required BettingUser User { get; set; }

        public required IList<Bet> ActiveBets { get; set; }
        public required IList<Bet> ClosedBets { get; set; }
	}
}
