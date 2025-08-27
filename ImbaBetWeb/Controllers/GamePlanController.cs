using ImbaBetWeb.Business;
using ImbaBetWeb.Model;
using ImbaBetWeb.ViewModels.GamePlan;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ImbaBetWeb.Controllers
{
    public class GamePlanController : Controller
    {
        private readonly GameManager _gameManager;
        private readonly BettingManager _bettingManager;
        private readonly UserManager<MyIdentityUser> _userManager;

        public GamePlanController(
            GameManager gameManager, 
            BettingManager bettingManager,
            UserManager<MyIdentityUser> userManager)
        {
            _gameManager = gameManager;
            _bettingManager = bettingManager;
            _userManager = userManager;
        }

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
                ActiveBets = await _bettingManager.GetActiveBetsForMatchAsync(matchId),
                ClosedBets = await _bettingManager.GetClosedBetsForMatchAsync(matchId)
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
