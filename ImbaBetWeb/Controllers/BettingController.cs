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
    public class BettingController : Controller
    {
        private readonly UserManager<MyIdentityUser> _userManager;
        private readonly BettingManager _bettingManager;
        private readonly CommunityManager _communityManager;
        private readonly DatabaseManager _databaseManager;

        public BettingController(
            UserManager<MyIdentityUser> userManager, 
            BettingManager bettingManager,
            CommunityManager communityManager,
            DatabaseManager databaseManager)
        {
            _userManager = userManager;
            _bettingManager = bettingManager;
            _communityManager = communityManager;
            this._databaseManager = databaseManager;
        }

        public async Task<IActionResult> Leaderboards()
        {
            var identityUser = await _userManager.GetUserAsync(User);
            Player? player = null;

            if (identityUser != null)
            {
                player = await _databaseManager.GetPlayerAsync(identityUser.PlayerId);
            }           

            var userRanking = await _bettingManager.GetPlayerRankingAsync();
            var communityRanking = await _bettingManager.GetCommunityRankingAsync();
            var internalRanking = player?.MemberOfCommunityId.HasValue ?? false ? await _bettingManager.GetPlayerRankingOfCommunityAsync(player.MemberOfCommunityId.Value) : null;

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
            var identityUser = await _userManager.GetUserAsync(User);
            if (identityUser == null)
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }

            var player = await _databaseManager.GetPlayerAsync(identityUser.PlayerId);

            var vm = new MyBetsViewModel()
            {
                OpenBets = await _bettingManager.GetOpenBetsOfPlayerAsync(player),
                ActiveBets = await _bettingManager.GetActiveBetsOfPlayerAsync(player),
                ClosedBets = await _bettingManager.GetClosedBetsOfPlayerAsync(player)
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
                var success = await _bettingManager.UpdateBetsAsync(vm.OpenBets);
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
