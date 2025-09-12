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
        BettingManager bettingManager) : ImbaBetControllerBase(userManager, playerManager)
    {

        private readonly BettingManager _bettingManager = bettingManager;

        public async Task<IActionResult> Profile(int playerId)
        {
            var userResolve = await TryResolveUserAsync();
            if (!userResolve.Success)
            {
                return RedirectToAction("Index", "Home");
            }

            var activeBets = await _bettingManager.GetActiveBetsOfPlayerAsync(userResolve.Player!);
			var closedBets = await _bettingManager.GetClosedBetsOfPlayerAsync(userResolve.Player!);

            var vm = new ProfileViewModel()
            {
                User = userResolve.User!,
                Player = userResolve.Player!,
                ClosedBets = closedBets,
                ActiveBets = activeBets
            };

			return View(vm);
        }
    }
}
