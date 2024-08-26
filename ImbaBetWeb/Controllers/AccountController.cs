using ImbaBetWeb.Logic;
using ImbaBetWeb.Logic.Extensions;
using ImbaBetWeb.Models;
using ImbaBetWeb.Models.Consts;
using ImbaBetWeb.ViewModels.Account;
using ImbaBetWeb.ViewModels.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ImbaBetWeb.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
		private readonly BettingManager _bettingManager;
        private readonly DatabaseManager _databaseManager;

        public AccountController(
            UserManager<ApplicationUser> userManager, 
            BettingManager bettingManager,
            DatabaseManager databaseManager)
        {
            _userManager = userManager;
			_bettingManager = bettingManager;
            _databaseManager = databaseManager;
        }

        public async Task<IActionResult> Profile(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var activeBets = await _bettingManager.GetActiveBetsForUserAsync(user);
			var closedBets = await _bettingManager.GetClosedBetsForUserAsync(user);

            var vm = new ProfileViewModel()
            {
                User = user,
                ClosedBets = closedBets,
                ActiveBets = activeBets
            };

			return View(vm);
        }
    }
}
