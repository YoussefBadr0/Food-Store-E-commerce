using FoodProject.Models;
using FoodProject.Services.Interfaces;
using FoodProject.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace FoodProject.Controllers
{
    [AllowAnonymous]
    public class PasswordController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IEmailService _emailService;

        public PasswordController(
            UserManager<AppUser> userManager,
            IEmailService emailService)
        {
            _userManager = userManager;
            _emailService = emailService;
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user != null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                var encodedToken = WebEncoders.Base64UrlEncode(
                    Encoding.UTF8.GetBytes(token));

                var resetLink = Url.Action(
                    action: nameof(ResetPassword),
                    controller: "Password",
                    values: new
                    {
                        email = model.Email,
                        token = encodedToken
                    },
                    protocol: Request.Scheme);

                var htmlBody = BuildResetPasswordEmailBody(
                    user.FirstName,
                    resetLink!);

                await _emailService.SendAsync(
                    user.Email!,
                    $"{user.FirstName} {user.LastName}",
                    "Reset Your Password - Food Store",
                    htmlBody);
            }

            return RedirectToAction(nameof(ForgotPasswordConfirmation));
        }

        [HttpGet]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword(string email, string token)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token))
                return BadRequest();

            var model = new ResetPasswordViewModel
            {
                Email = email,
                Token = token
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
                return RedirectToAction(nameof(ResetPasswordConfirmation));

            var decodedToken = Encoding.UTF8.GetString(
                WebEncoders.Base64UrlDecode(model.Token));

            var result = await _userManager.ResetPasswordAsync(
                user,
                decodedToken,
                model.NewPassword);

            if (result.Succeeded)
                return RedirectToAction(nameof(ResetPasswordConfirmation));

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        private static string BuildResetPasswordEmailBody(string firstName, string resetLink)
        {
            return $@"
<div style='font-family:Arial,sans-serif;max-width:520px;margin:auto;border:1px solid #e0e0e0;border-radius:10px;overflow:hidden;'>
    <div style='background:#2d6a4f;padding:24px;text-align:center;'>
        <h1 style='color:#fff;margin:0;font-size:22px;'>Food Store</h1>
    </div>
    <div style='padding:32px;'>
        <h2 style='color:#2d6a4f;margin-top:0;'>Reset Your Password</h2>
        <p style='color:#444;'>Hi <strong>{firstName}</strong>,</p>
        <p style='color:#444;'>
            We received a request to reset your password.
            Click the button below to choose a new one:
        </p>
        <div style='text-align:center;margin:32px 0;'>
            <a href='{resetLink}'
               style='background:#2d6a4f;color:#fff;padding:14px 32px;border-radius:6px;text-decoration:none;font-weight:bold;font-size:15px;'>
                Reset Password
            </a>
        </div>
        <p style='color:#888;font-size:13px;'>
            This link expires in <strong>1 hour</strong>.
        </p>
        <p style='color:#888;font-size:13px;'>
            If you didn't request this, you can safely ignore this email.
        </p>
    </div>
    <div style='background:#f5f5f5;padding:16px;text-align:center;'>
        <p style='color:#aaa;font-size:12px;margin:0;'>
            &copy; {DateTime.Now.Year} Food Store. All rights reserved.
        </p>
    </div>
</div>";
        }
    }
}
