using ImbaBetWeb.Logic;
using ImbaBetWeb.Logic.Extensions;
using ImbaBetWeb.Models;
using ImbaBetWeb.ViewModels.Orga;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ImbaBetWeb.Controllers
{
    [Authorize]
    public class OrgaController : Controller
    {
        private readonly BettingManager _bettingManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly CommunityManager _communityManager;

        public OrgaController(
            BettingManager bettingManager, 
            UserManager<ApplicationUser> userManager,
            CommunityManager communityManager)
        {
            _bettingManager = bettingManager;
            _userManager = userManager;
            _communityManager = communityManager;
        }


        public async Task<IActionResult> MyCommunity()
        {
            var communities = await _communityManager.Communities.ToListAsync();
            var user = await _userManager.GetUserAsync(User);
            if(user == null)
            {
                return RedirectToAction("Error", "Home");
            }

            var vm = new MyCommunityViewModel()
            {
                Communities = communities,
                User = user
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCommunity(string communityName)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Error", "Home");
            }

            await _communityManager.CreateCommunityAsync(user, communityName);

            return RedirectToAction(nameof(MyCommunity));
        }

        [HttpPost]
        public async Task<IActionResult> JoinCommunity()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Error", "Home");
            }

            var dropdownValue = Request.Form[OrgaConsts.Dropdown_CommunitySelection];
            if (string.IsNullOrEmpty(dropdownValue))
            {
                return RedirectToAction("Error", "Home");
            }
            var communityId = int.Parse(dropdownValue!);

            var wasSuccessful = await _communityManager.JoinCommunityAsync(user, communityId);
            if(wasSuccessful)
            {
                this.SetSuccessAlert($"Community joined successfully.");
            }

            return RedirectToAction(nameof(MyCommunity));
        }

        [HttpGet]
        public async Task<IActionResult> LeaveCommunity()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Error", "Home");
            }
            var community = user.MemberOfCommunity;
            var isOwner = user.OwnerOfCommunity == user.MemberOfCommunity;

            var wasSuccessful = isOwner ? await _communityManager.DeleteCommunityOfUserAsync(user) : await _communityManager.LeaveCommunityAsync(user);
            if (wasSuccessful)
            {
                this.SetSuccessAlert($"Community ({community!.Name}) has been left successfully.");
            }

            return RedirectToAction(nameof(MyCommunity));
        }

        [HttpGet]
        [Route("Orga/KickMember/{userId}")]
        public async Task<IActionResult> KickMember(string userId)
        {
            var user = await _userManager.GetUserAsync(User);
            var userToBeKicked = await _userManager.FindByIdAsync(userId);
            var community = user?.OwnerOfCommunity;

            if(community == null || user == null || userToBeKicked == null)
            {
                this.SetErrorAlert("Error while kicking user");
                return RedirectToAction(nameof(MyCommunity));
            }

            await _communityManager.KickMemberAsync(community.Id, userToBeKicked.Id);

            this.SetSuccessAlert($"{userToBeKicked.UserName} has been kicked from Community.");

            return RedirectToAction(nameof(MyCommunity));
        }

        [HttpGet]
        [Route("Orga/PromoteToOwner/{userId}")]
        public async Task<IActionResult> PromoteToOwner(string userId)
        {
            var user = await _userManager.GetUserAsync(User);
            var newOwner = await _userManager.FindByIdAsync(userId);
            var community = user?.OwnerOfCommunity;

            if (community == null || newOwner == null)
            {
                return RedirectToAction(nameof(MyCommunity));
            }

            await _communityManager.PromoteToOwnerAsync(user!.OwnerOfCommunity!.Id, newOwner.Id);

            this.SetSuccessAlert($"{newOwner.UserName} has been promoted to owner.");

            return RedirectToAction(nameof(MyCommunity));
        }

    }
}
