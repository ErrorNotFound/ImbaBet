using ImbaBetWeb.Business;
using ImbaBetWeb.Model;
using ImbaBetWeb.ViewModels.MatchPlan;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ImbaBetWeb.Controllers
{
    public class MatchPlanController(
        GameManager gameManager,
        BettingManager bettingManager,
        UserManager<MyIdentityUser> userManager,
        PlayerManager playerManager) : ImbaBetControllerBase(userManager, playerManager)
    {
        private readonly GameManager _gameManager = gameManager;
        private readonly BettingManager _bettingManager = bettingManager;

        public IActionResult Index()
        {
            return RedirectToAction(nameof(Teams));
        }

        public async Task<IActionResult> Teams()
        {
            var ranking = await _gameManager.GetTeamRankingAsync();

            return View(ranking);
        }

        public async Task<IActionResult> Match(int matchId)
        {
            var match = await _gameManager.GetMatchByIdAsync(matchId);
            if (match == null)
            {
                return RedirectToAction(nameof(Teams));
            }

            var vm = new MatchViewModel()
            {
                Match = match,
                ActiveBets = await _bettingManager.GetActiveBetsOfMatchAsync(match),
                ClosedBets = await _bettingManager.GetClosedBetsOfMatchAsync(match)
            };

            return View(vm);
        }

        public async Task<IActionResult> Matches()
        {
            var matchplan = await _gameManager.GetMatchplanAsync();

            return View(matchplan);
        }

        public async Task<IActionResult> Groups()
        {
            var matchGroupRanking = await _gameManager.GetGroupRankingAsync();

            return View(matchGroupRanking);
        }
    }
}
