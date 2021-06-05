﻿using System.Threading.Tasks;

using CRM.Core.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace CRM.Server.Areas.Account.Pages.Manage
{
	public class ResetAuthenticatorModel : PageModel
	{
		private readonly UserManager<ApplicationUser> userManager;
		private readonly SignInManager<ApplicationUser> signInManager;
		private readonly ILogger<ResetAuthenticatorModel> logger;

		public ResetAuthenticatorModel(
			UserManager<ApplicationUser> userManager,
			SignInManager<ApplicationUser> signInManager,
			ILogger<ResetAuthenticatorModel> logger)
		{
			this.userManager = userManager;
			this.signInManager = signInManager;
			this.logger = logger;
		}

		[TempData]
		public string StatusMessage { get; set; }

		public async Task<IActionResult> OnGet()
		{
			ApplicationUser? user = await userManager.GetUserAsync(User);
			if (user == null)
			{
				return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
			}

			return Page();
		}

		public async Task<IActionResult> OnPostAsync()
		{
			ApplicationUser? user = await userManager.GetUserAsync(User);
			if (user == null)
			{
				return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
			}

			await userManager.SetTwoFactorEnabledAsync(user, false);
			await userManager.ResetAuthenticatorKeyAsync(user);
			logger.LogInformation("User with ID '{UserId}' has reset their authentication app key.", user.Id);

			await signInManager.RefreshSignInAsync(user);
			StatusMessage = "Your authenticator app key has been reset, you will need to configure your authenticator app using the new key.";

			return RedirectToPage("./EnableAuthenticator");
		}
	}
}