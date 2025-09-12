using ImbaBetWeb.Business;
using ImbaBetWeb.Business.Extensions;
using ImbaBetWeb.Model;
using ImbaBetWeb.Validation;
using ImbaBetWeb.ViewModels.Betting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ImbaBetWeb.Controllers
{
    public class BettingController(
        UserManager<MyIdentityUser> userManager,
        BettingManager bettingManager,
        CommunityManager communityManager,
        DatabaseManager databaseManager,
        PlayerManager playerManager) : ImbaBetControllerBase(userManager, playerManager)
    {
        private readonly BettingManager _bettingManager = bettingManager;
        private readonly CommunityManager _communityManager = communityManager;
        private readonly DatabaseManager _databaseManager = databaseManager;

        public async Task<IActionResult> Leaderboards()
        {
            var userResolve = await TryResolveUserAsync();

            var userRanking = await _bettingManager.GetPlayerRankingAsync();
            var communityRanking = await _bettingManager.GetCommunityRankingAsync();
            var internalRanking = userResolve.Player?.MemberOfCommunityId.HasValue ?? false ? await _bettingManager.GetPlayerRankingOfCommunityAsync(userResolve.Player.MemberOfCommunityId.Value) : null;

            var vm = new LeaderboardsViewModel()
            {
                PlayerRanking = userRanking,
                CommunityRanking = communityRanking,
                CommunityInternalRanking = internalRanking
            };

            return View(vm);
        }

        public async Task<IActionResult> Communities()
        {
            var c = await _communityManager.GetCommunitiesAsync();

            return View(c);
        }

        [Authorize]
        public async Task<IActionResult> MyBets()
        {
            var userResolve = await TryResolveUserAsync();
            if (!userResolve.Success)
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }

            var vm = new MyBetsViewModel()
            {
                OpenBets = [.. (await _bettingManager.GetOpenBetsOfPlayerAsync(userResolve.Player!))],
                ActiveBets = [.. (await _bettingManager.GetActiveBetsOfPlayerAsync(userResolve.Player!))],
                ClosedBets = [.. (await _bettingManager.GetClosedBetsOfPlayerAsync(userResolve.Player!))]
            };

            return View(vm);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> MyBets(MyBetsViewModel vm)
        {
            var validator = new MyBetsViewModelValidator();
            var validationResult = validator.Validate(vm);

            if (validationResult.IsValid)
            {
                var success = await _bettingManager.CreateAndUpdateBetsAsync(vm.OpenBets);
                if (success)
                {
                    this.SetSuccessAlert("Your bets have been saved.");
                }
                else
                {
                    this.SetErrorAlert("Error while saving your bets.");
                }
            }
            else
            {
                foreach(var error in validationResult.Errors)
                {
                    this.SetErrorAlert(error.ErrorMessage);
                }                
            }

            return RedirectToAction(nameof(MyBets));
        }
    }
}
