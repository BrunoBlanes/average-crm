﻿using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

using CRM.Core.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CRM.Server.Areas.Account.Pages.Manage
{
	public partial class IndexModel : PageModel
	{
		private readonly UserManager<ApplicationUser> userManager;
		private readonly SignInManager<ApplicationUser> signInManager;

		[TempData]
		public string? StatusMessage { get; set; }
		public string? Username { get; set; }

		[Phone]
		[BindProperty]
		[Display(Name = "Phone number")]
		public string PhoneNumber { get; set; }

		public IndexModel(
			UserManager<ApplicationUser> userManager,
			SignInManager<ApplicationUser> signInManager)
		{
			this.userManager = userManager;
			this.signInManager = signInManager;
			PhoneNumber = string.Empty;
		}

		private async Task LoadAsync(ApplicationUser user)
		{
			Username = await userManager.GetUserNameAsync(user);
			PhoneNumber = await userManager.GetPhoneNumberAsync(user);
		}

		public async Task<IActionResult> OnGetAsync()
		{
			if (await userManager.GetUserAsync(User) is ApplicationUser user)
			{
				await LoadAsync(user);
				return Page();
			}

			return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
		}

		public async Task<IActionResult> OnPostAsync()
		{
			ApplicationUser? user = await userManager.GetUserAsync(User);
			
			if (user is null)
			{
				return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
			}

			if (ModelState.IsValid)
			{
				var phoneNumber = await userManager.GetPhoneNumberAsync(user);

				if (PhoneNumber != phoneNumber)
				{
					IdentityResult? setPhoneResult = await userManager.SetPhoneNumberAsync(user, PhoneNumber);

					if (setPhoneResult.Succeeded is false)
					{
						StatusMessage = "Unexpected error when trying to set phone number.";
						return RedirectToPage();
					}
				}

				await signInManager.RefreshSignInAsync(user);
				StatusMessage = "Your profile has been updated";
				return RedirectToPage();
			}

			await LoadAsync(user);
			return Page();
		}
	}
}