using ImbaBetWeb.Business;
using ImbaBetWeb.Model;
using ImbaBetWeb.ViewModels.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ImbaBetWeb.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<MyIdentityUser> _userManager;
		private readonly BettingManager _bettingManager;
        private readonly DatabaseManager _databaseManager;

        public AccountController(
            UserManager<MyIdentityUser> userManager, 
            BettingManager bettingManager,
            DatabaseManager databaseManager)
        {
            _userManager = userManager;
			_bettingManager = bettingManager;
            _databaseManager = databaseManager;
        }

        public async Task<IActionResult> Profile(string userId)
        {
            var idUser = await _userManager.FindByIdAsync(userId);
            if (idUser == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var bettingUser = await _databaseManager.GetUserAsync(idUser.BettingUserId);
            if (bettingUser == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var activeBets = await _bettingManager.GetActiveBetsOfUserAsync(bettingUser);
			var closedBets = await _bettingManager.GetClosedBetsOfUserAsync(bettingUser);

            var vm = new ProfileViewModel()
            {
                User = idUser,
                BettingUser = bettingUser,
                ClosedBets = closedBets,
                ActiveBets = activeBets
            };

			return View(vm);
        }
    }
}
