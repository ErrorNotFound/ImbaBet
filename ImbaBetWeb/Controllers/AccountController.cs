using ImbaBetWeb.Business;
using ImbaBetWeb.Model;
using ImbaBetWeb.ViewModels.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ImbaBetWeb.Controllers
{
    public class AccountController(
        UserManager<MyIdentityUser> userManager,
        PlayerManager playerManager,
        BettingManager bettingManager,
        IdentityManager identityManager) : ImbaBetControllerBase(userManager, playerManager)
    {
        private readonly BettingManager _bettingManager = bettingManager;
        private readonly IdentityManager _identityManager = identityManager;

        public async Task<IActionResult> Profile(int playerId)
        {
            var player = await _playerManager.GetPlayerAsync(playerId);
            var user = _identityManager.GetIdentityUser(playerId);

            if (player == null || user == null)
            {
                return RedirectToAction("Error", "Home");
            }

            var activeBets = await _bettingManager.GetActiveBetsOfPlayerAsync(player);
			var closedBets = await _bettingManager.GetClosedBetsOfPlayerAsync(player);

            var vm = new ProfileViewModel()
            {
                User = user,
                Player = player,
                ClosedBets = closedBets,
                ActiveBets = activeBets
            };

			return View(vm);
        }
    }
}
