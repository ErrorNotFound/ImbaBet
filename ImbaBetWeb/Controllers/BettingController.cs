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
            BettingUser? bettingUser = null;

            if (identityUser != null)
            {
                bettingUser = await _databaseManager.GetUserAsync(identityUser.BettingUserId);
            }           

            var userRanking = await _bettingManager.GetUserRankingAsync();
            var communityRanking = await _bettingManager.GetCommunityRankingAsync();
            var internalRanking = bettingUser?.MemberOfCommunityId.HasValue ?? false ? await _bettingManager.GetUserRankingOfCommunityAsync(bettingUser.MemberOfCommunityId.Value) : null;

            var vm = new LeaderboardsViewModel()
            {
                UserRanking = userRanking,
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

            var bettingUser = await _databaseManager.GetUserAsync(identityUser.BettingUserId);

            var vm = new MyBetsViewModel()
            {
                OpenBets = await _bettingManager.GetOpenBetsForUserAsync(bettingUser),
                ActiveBets = await _bettingManager.GetActiveBetsForUserAsync(bettingUser),
                ClosedBets = await _bettingManager.GetClosedBetsForUserAsync(bettingUser)
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
