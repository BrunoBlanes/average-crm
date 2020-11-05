﻿using System.Threading;
using System.Threading.Tasks;

using CRM.Core.Models;

using MimeKit;

namespace CRM.Server.Interfaces
{
	public interface ISmtpService
	{
		/// <summary>
		/// Attempts to send a message using the <see cref="MailKit.Net.Smtp.SmtpClient"/>.
		/// </summary>
		/// <param name="token">The <see cref="CancellationToken"/>.</param>
		/// <param name="message">The <see cref="MimeMessage"/> to be sent.</param>
		/// <returns>A <see cref="Task"/> that represents the message being sent.</returns>
		/// <exception cref="System.InvalidOperationException">Thrown when no recipients have been specified.</exception>
		/// <exception cref="MailKit.Security.AuthenticationException">Thrown when authentication using the supplied credentials has failed.
		/// </exception>
		/// <exception cref="System.NotSupportedException">Thrown when options was set to <see cref="MailKit.Security.SecureSocketOptions.StartTls"/>
		/// and the SMTP server does not support the STARTTLS extension.</exception>
		Task SendEmailAsync(MimeMessage message, CancellationToken token = default);

		/// <summary>
		/// Attempts to send a message with the account confirmation code using the <see cref="SendEmailAsync(MimeMessage, CancellationToken)"/> method.
		/// </summary>
		/// <param name="callbackUrl">The url for confirming the user account.</param>
		/// <param name="user">The <see cref="ApplicationUser"/> who's account confirmation is requested.</param>
		/// <param name="token">The <see cref="CancellationToken"/>.</param>
		/// <returns>A <see cref="Task"/> that represents the message being sent.</returns>
		Task SendAccountConfirmationEmailAsync(string callbackUrl, ApplicationUser user, CancellationToken token = default);
	}
}