﻿using Microsoft.AspNetCore.Authorization;
using System.Text;
using System.Threading.Tasks;
using CRM.Shared.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.Encodings.Web;

namespace CRM.Server.Areas.Identity.Pages.Account
{
	[AllowAnonymous]
	public class RegisterConfirmationModel : PageModel
	{
		private readonly UserManager<ApplicationUser> userManager;
		private readonly IEmailSender emailSender;

		public string? Email { get; set; }
		public bool DisplayConfirmAccountLink { get; set; }

		public RegisterConfirmationModel(UserManager<ApplicationUser> userManager, IEmailSender sender)
		{
			this.emailSender = sender;
			this.userManager = userManager;
		}

		public async Task<IActionResult> OnGetAsync(string email, string? returnUrl = null)
		{
			if (email is null) return RedirectToPage("/Index");
			ApplicationUser? user = await userManager.FindByEmailAsync(email);

			if (user is not null)
			{
				Email = email;
				var userId = await userManager.GetUserIdAsync(user);

				// Generates an account confirmation code
				var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
				code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
				var callbackUrl = Url.Page(
					"/Account/ConfirmEmail",
					pageHandler: null,
					values: new { area = "Identity", userId, code, returnUrl },
					protocol: Request.Scheme);

				// Sends an email to the user with the account confirmation code
				await emailSender.SendEmailAsync(Email, "Confirm your email",
					$"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
				return Page();
			}

			return NotFound($"Unable to load user with email '{email}'.");
		}
	}
}