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
        private readonly PlayerManager _playerManager;
        private readonly BettingManager _bettingManager;
        private readonly DatabaseManager _databaseManager;

        public AccountController(
            UserManager<MyIdentityUser> userManager, 
            PlayerManager playerManager,
            BettingManager bettingManager,
            DatabaseManager databaseManager)
        {
            _userManager = userManager;
            _playerManager = playerManager;
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

            var player = await _playerManager.GetPlayerAsync(idUser.PlayerId);
            if (player == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var activeBets = await _bettingManager.GetActiveBetsOfPlayerAsync(player);
			var closedBets = await _bettingManager.GetClosedBetsOfPlayerAsync(player);

            var vm = new ProfileViewModel()
            {
                User = idUser,
                Player = player,
                ClosedBets = closedBets,
                ActiveBets = activeBets
            };

			return View(vm);
        }
    }
}
