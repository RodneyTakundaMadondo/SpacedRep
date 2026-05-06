// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using StudyReminder.Services;

namespace StudyReminder.Areas.Identity.Pages.Account
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly Services.EmailTemplate _emailTemplate;

        public ForgotPasswordModel(UserManager<IdentityUser> userManager, IEmailService emailService, EmailTemplate emailTemplate)
        {
            _userManager = userManager;
            _emailService = emailService;
            _emailTemplate = emailTemplate;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    TempData["ErrorMessage"] = "Invalid email address";
                    return Page();
                    //return RedirectToPage("./ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please
                // visit https://go.microsoft.com/fwlink/?LinkID=532713
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ResetPassword",
                    pageHandler: null,
                    values: new { area = "Identity", code, email = Input.Email},
                    protocol: Request.Scheme);

                var htmlBody = _emailTemplate.GetTemplate(HtmlEncoder.Default.Encode(callbackUrl),"forgotPassword.html", "resetLink");

                await _emailService.SendEmailAsync("madondotakundaoriginal@gmail.com",
                    Input.Email,
                    "Reset Password",
                   htmlBody);

                //return RedirectToPage("./ForgotPasswordConfirmation");
                TempData["EmailSuccessHeading"] = "Email Sent";
                TempData["EmailMessage"] = "Your password reset email has been sent to you. Please check your Inbox.";
                return Page();
            }

            return Page();
        }
    }
}
