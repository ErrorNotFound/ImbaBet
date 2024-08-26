using ImbaBetWeb.Logic;
using ImbaBetWeb.Logic.Extensions;
using ImbaBetWeb.Models;
using ImbaBetWeb.ViewModels.Betting;
using ImbaBetWeb.ViewModels.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ImbaBetWeb.Controllers
{
    public class BettingController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly BettingManager _bettingManager;
        private readonly CommunityManager _communityManager;

        public BettingController(
            UserManager<ApplicationUser> userManager, 
            BettingManager bettingManager,
            CommunityManager communityManager)
        {
            _userManager = userManager;
            _bettingManager = bettingManager;
            _communityManager = communityManager;
        }

        public async Task<IActionResult> Leaderboards()
        {
            var user = await _userManager.GetUserAsync(User);

            var userRanking = await _bettingManager.GetUserRankingAsync();
            var communityRanking = await _bettingManager.GetCommunityRankingAsync();
            var internalRanking = user?.MemberOfCommunityId.HasValue ?? false ? await _bettingManager.GetUserRankingOfCommunityAsync(user.MemberOfCommunityId.Value) : null;

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
            var c = await _communityManager.Communities.ToListAsync();

            return View(c);
        }

        [Authorize]
        public async Task<IActionResult> MyBets()
        {
            var currentApplicationUser = await _userManager.GetUserAsync(User);
            if (currentApplicationUser == null)
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
            
            var vm = new MyBetsViewModel()
            {
                OpenBets = await _bettingManager.GetOpenBetsForUserAsync(currentApplicationUser),
                ActiveBets = await _bettingManager.GetActiveBetsForUserAsync(currentApplicationUser),
                ClosedBets = await _bettingManager.GetClosedBetsForUserAsync(currentApplicationUser)
            };

            return View(vm);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> MyBets(MyBetsViewModel vm)
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

            return RedirectToAction(nameof(MyBets));
        }
    }
}
