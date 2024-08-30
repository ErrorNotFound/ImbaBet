using ImbaBetWeb.Models;
using ImbaBetWeb.ViewModels.DTO;

namespace ImbaBetWeb.ViewModels.Account
{
	public class ProfileViewModel
	{
		public required ApplicationUser User { get; set; }

        public required IList<Bet> ActiveBets { get; set; }
        public required IList<Bet> ClosedBets { get; set; }
	}
}
