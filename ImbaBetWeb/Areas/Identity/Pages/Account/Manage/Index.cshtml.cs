// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using ImbaBetWeb.Logic;
using ImbaBetWeb.Logic.Extensions;
using ImbaBetWeb.Logic.Helper;
using ImbaBetWeb.Models;
using ImbaBetWeb.Validation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace ImbaBetWeb.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly DatabaseManager _databaseManager;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            DatabaseManager databaseManager,
            IConfiguration configuration,
            IWebHostEnvironment webHostEnvironment)
        {
            
            _userManager = userManager;
            _signInManager = signInManager;
            _databaseManager = databaseManager;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public UsernameChangeModel UsernameChange { get; set; }

        [BindProperty]
        public FileUploadModel FileUpload { get; set; }

        public class UsernameChangeModel
        {
            [Required]
            [StringLength(20, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 3)]
            [Display(Name = "Username")]
            [DataType(DataType.Text)]
            public string Username { get; set; }
        }

        public class FileUploadModel
        {
            [Required]
            [Display(Name = "File")]
            public IFormFile FormFile { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            UsernameChange = new UsernameChangeModel
            {
                Username = user.UserName
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            
            if (!this.IsValid(UsernameChange))
            {
                return Page();
            }

            var existingUsernames = _userManager.Users.Select(x => x.UserName).ToList();
            var validator = new UsernameValidator(existingUsernames);
            var validationResult = validator.Validate(UsernameChange.Username);
            if (validationResult.IsValid)
            {
                if (UsernameChange.Username != user.UserName && user.RemainingRenames > 0)
                {
                    var setUserNameResult = await _userManager.SetUserNameAsync(user, UsernameChange.Username);
                    if (!setUserNameResult.Succeeded)
                    {
                        StatusMessage = "Unexpected error when trying to set username.";
                        return RedirectToPage();
                    }
                    user.RemainingRenames -= 1;
                    await _userManager.UpdateAsync(user);
                }

                await _signInManager.RefreshSignInAsync(user);
                StatusMessage = "Your profile has been updated";
                return RedirectToPage();
            }
            else
            {
                foreach (var error in validationResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.ErrorMessage);
                }

                return Page();
            }
        }

        public async Task<IActionResult> OnPostDeletePictureAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await _databaseManager.DeleteProfilePicture(user.Id);

            StatusMessage = "Your profile picture has been deleted";

            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnPostUploadAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var allowedExtensions = _configuration["ProfilePictureUpload:UploadAllowedExtensions"].Split(';');
            var fileSizeLimit = int.Parse(_configuration["ProfilePictureUpload:FileSizeLimitInBytes"]);

            var formFileContent = await FileHelpers.ProcessFormFile<FileUploadModel>(
                    FileUpload.FormFile, ModelState, allowedExtensions,
                    fileSizeLimit);

            if (!this.IsValid(FileUpload))
            {
                StatusMessage = "Please correct the form.";

                return Page();
            }

            var trustedFileNameForFileStorage = Path.GetRandomFileName();
            var relativePath = _configuration["ProfilePictureUpload:UploadDirectory"] + trustedFileNameForFileStorage + Path.GetExtension(FileUpload.FormFile.FileName);
            var filePathAbsolute = _webHostEnvironment.WebRootPath + relativePath;

            using (var fileStream = System.IO.File.Create(filePathAbsolute))
            {
                await fileStream.WriteAsync(formFileContent); 
            }

            await _databaseManager.DeleteProfilePicture(user.Id);

            user.ProfilePicturePath = relativePath;
            await _userManager.UpdateAsync(user);

            return RedirectToPage("./Index");
        }
    }

}

