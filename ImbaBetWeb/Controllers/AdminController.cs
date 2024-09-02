using ImbaBetWeb.Logic;
using ImbaBetWeb.Logic.Extensions;
using ImbaBetWeb.Models;
using ImbaBetWeb.Models.Consts;
using ImbaBetWeb.Validation;
using ImbaBetWeb.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace ImbaBetWeb.Controllers
{
    [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Editor}")]
    public class AdminController : Controller
    {
        private readonly BettingManager _bettingManager;
        private readonly GameManager _gameManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly DatabaseManager _databaseManager;
        private readonly CommunityManager _communityManager;
        private readonly SettingsManager _settingsManager;
        private readonly MatchPlanImportService _matchPlanImportService;

        public AdminController(
            BettingManager bettingManager,
            GameManager gameManager,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            DatabaseManager databaseManager,
            CommunityManager communityManager,
            SettingsManager settingsManager,
            MatchPlanImportService matchPlanImportService)
        {
            _bettingManager = bettingManager;
            _gameManager = gameManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _databaseManager = databaseManager;
            _communityManager = communityManager;
            _settingsManager = settingsManager;
            _matchPlanImportService = matchPlanImportService;
        }

        public async Task<IActionResult> Matches()
        {
            var vm = new MatchesViewModel()
            {
                MatchGroups = await _gameManager.GetMatchGroupsAsync(),
                Matches = await _gameManager.GetMatchesAsync(),
                Teams = await _gameManager.GetTeamsAsync()
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Matches(MatchesViewModel vm)
        {
            var validator = new MatchesViewModelValidator();
            var validationResult = validator.Validate(vm);

            if(validationResult.IsValid)
            {
                await _gameManager.UpdateMatchesAsync(vm.Matches);
                await _bettingManager.UpdatePointsAsync();

                this.SetSuccessAlert("Matches have been saved and points updated.");
            }
            else
            {
                foreach (var error in validationResult.Errors)
                {
                    this.SetErrorAlert(error.ErrorMessage);
                }
            }

            return RedirectToAction(nameof(Matches));
        }

        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> Accounts()
        {
            var users = await _userManager.Users.AsNoTracking().ToListAsync();
            var communities = await _communityManager.Communities.ToListAsync();

            var dtos = users.Select(async u => new UserDTO()
            {
                Id = u.Id,
                Username = u?.UserName ?? "Username not found",
                Email = u?.Email ?? "Email not found",
                MemberOfCommunityId = u!.MemberOfCommunityId,
                IsAdmin = await _userManager.IsInRoleAsync(u, UserRoles.Admin),
                IsEditor = await _userManager.IsInRoleAsync(u, UserRoles.Editor)
            }).Select(x => x.Result).ToList();

            return View(new AccountsViewModel()
            {
                Users = dtos,
                Communities = communities
            });
        }

        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> Settings()
        {
            var settings = await _settingsManager.GetAllSettingsAsync();

            return View(settings);
        }

        [HttpGet]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> SaveSetting(string key, string value)
        {
            var success = await _settingsManager.SetSettingAsync(key, value);
            if(success)
            {
                this.SetSuccessAlert($"Setting {key} has been saved.");
            }
            else
            {
                this.SetErrorAlert($"Setting {key} has not been saved.");
            }
            
            return RedirectToAction(nameof(Settings));
        }

        [HttpGet]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> ResetSetting(string key)
        {
            var success = await _settingsManager.ResetSettingAsync(key);
            if (success)
            {
                this.SetSuccessAlert($"Setting {key} has been resetted.");
            }
            else
            {
                this.SetErrorAlert($"Setting {key} has not been resetted.");
            }

            return RedirectToAction(nameof(Settings));
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> UpdateUsers(AccountsViewModel vm)
        {
            var success = true;
            foreach (var user in vm.Users) 
            {
                var dbUser = await _userManager.FindByIdAsync(user.Id);
                if(dbUser != null)
                {
                    success &= await _communityManager.UpdateCommunityMembershipAsync(user.Id, user.MemberOfCommunityId);
                    success &= await _databaseManager.UpdateRolesAsync(user.Id, user.IsAdmin, user.IsEditor);
                }                
            }

            if (success)
            {
                this.SetSuccessAlert($"Users have been updated.");
            }
            else
            {
                this.SetErrorAlert($"Users have not been updated.");
            }

            return RedirectToAction(nameof(Accounts));
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> UpdateCommunities(AccountsViewModel vm)
        {
            if(vm.Communities == null)
            {
                return RedirectToAction(nameof(Accounts));
            }

            await _communityManager.UpdateCommunitiesAsync(vm.Communities);

            this.SetSuccessAlert($"Communities have been updated.");

            return RedirectToAction(nameof(Accounts));
        }


        [HttpGet]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            await _databaseManager.DeleteUserAsync(userId);

            this.SetSuccessAlert($"User with ID {userId} has been deleted.");

            return RedirectToAction(nameof(Accounts));
        }

        [HttpGet]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> DeleteProfilePicture(string userId)
        {
            await _databaseManager.DeleteProfilePicture(userId);

            this.SetSuccessAlert($"Profile picture of {userId} has been deleted.");

            return RedirectToAction(nameof(Accounts));
        }


        [HttpGet]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> DeleteCommunity(int communityId)
        {
            if(await _communityManager.DeleteCommunityAsync(communityId))
            {
                this.SetSuccessAlert($"Community with ID {communityId} has been deleted.");
            }
            else
            {
                this.SetErrorAlert($"Community with ID {communityId} could not be deleted.");
            }

            return RedirectToAction(nameof(Accounts));
        }

        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> DeleteGameData()
        {
            await _databaseManager.DeleteGameDataAsync();

            this.SetSuccessAlert("Game data has been deleted.");

            return RedirectToAction(nameof(Settings));
        }

        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> SeedTestdata()
        {
            await _databaseManager.SeedTestDataAsync();

            this.SetSuccessAlert("Database has been seeded.");

            return RedirectToAction(nameof(Settings));
        }

        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> UpdateLeaderboards()
        {
            await _bettingManager.UpdatePointsAsync();

            this.SetSuccessAlert("Leaderboards have been updated.");

            return RedirectToAction(nameof(Settings));
        }

        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> ImportMatchplan()
        {
            var vm = new ImportMatchplanViewModel
            {
                Templates = await _matchPlanImportService.GetTemplatesAsync(),
                MatchPlanInput = string.Empty,
                ValidationErrors = new List<string>()
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> ImportMatchplan(ImportMatchplanViewModel vm)
        {
            var matchplan = vm.MatchPlanInput;
            var validationResult = _matchPlanImportService.ValidateMatchPlanXmlAsync(matchplan);
            if (validationResult.isValid)
            {
                await _databaseManager.DeleteGameDataAsync();
                await _matchPlanImportService.ImportAsync(matchplan);
                this.SetSuccessAlert("MatchPlan has been imported.");
            }
            else
            {
                this.SetErrorAlert("MatchPlan is not valid.");
                return View(new ImportMatchplanViewModel
                {
                    MatchPlanInput = vm.MatchPlanInput,
                    Templates = await _matchPlanImportService.GetTemplatesAsync(),
                    ValidationErrors = validationResult.ValidationErrors
                });
            }

            return RedirectToAction(nameof(ImportMatchplan));
        }

        [AllowAnonymous]
        public async Task<IActionResult> CreateAdmin()
        {
            await _databaseManager.CreateAdminAsync();

            return RedirectToAction(nameof(Accounts));
        }

    }
}
