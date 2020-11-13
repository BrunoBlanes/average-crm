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
		private readonly ILogger<ResetAuthenticatorModel> logger;
		private readonly UserManager<ApplicationUser> userManager;
		private readonly SignInManager<ApplicationUser> signInManager;

		[TempData]
		public string? StatusMessage { get; set; }

		public ResetAuthenticatorModel(
			SignInManager<ApplicationUser> signInManager,
			UserManager<ApplicationUser> userManager,
			ILogger<ResetAuthenticatorModel> logger)
		{
			this.logger = logger;
			this.userManager = userManager;
			this.signInManager = signInManager;
		}

		public async Task<IActionResult> OnGet()
		{
			ApplicationUser? user = await userManager.GetUserAsync(User);
			return user is null
				? NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.")
				: (IActionResult)Page();
		}

		public async Task<IActionResult> OnPostAsync()
		{
			ApplicationUser? user = await userManager.GetUserAsync(User);
			
			if (user is null)
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