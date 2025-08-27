using ImbaBetWeb.Model;
using ImbaBetWeb.ViewModels.DTO;

namespace ImbaBetWeb.ViewModels.Account
{
	public class ProfileViewModel
	{
		public required MyIdentityUser User { get; set; }

        public required BettingUser BettingUser { get; set; }

        public required IEnumerable<Bet> ActiveBets { get; set; }
        public required IEnumerable<Bet> ClosedBets { get; set; }
	}
}
