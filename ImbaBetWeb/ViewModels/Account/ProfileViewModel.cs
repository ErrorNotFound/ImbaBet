using ImbaBetWeb.Models;
using ImbaBetWeb.ViewModels.DTO;

namespace ImbaBetWeb.ViewModels.Account
{
	public class ProfileViewModel
	{
		public ApplicationUser User { get; set; }

        public IList<Bet> ActiveBets { get; set; }
        public IList<Bet> ClosedBets { get; set; }
	}
}
