using ImbaBetWeb.Model;

namespace ImbaBetWeb.ViewModels.Account
{
	public class ProfileViewModel
	{
		public required MyIdentityUser User { get; set; }

        public required Player Player { get; set; }

        public required IEnumerable<Bet> ActiveBets { get; set; }
        public required IEnumerable<Bet> ClosedBets { get; set; }
	}
}
