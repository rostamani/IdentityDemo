using AutoMapper;
using IdentitySample.Repositories;
using IdentitySample.ViewModels.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentitySample.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signinManager;
        private readonly IMapper _mapper;
        private readonly IMessageSender _messageSender;

        public AccountController(UserManager<IdentityUser> userManager
            , IMapper mapper
            , SignInManager<IdentityUser> signinManager
            , IMessageSender messageSender)
        {
            _userManager = userManager;
            _mapper = mapper;
            _signinManager = signinManager;
            _messageSender = messageSender;
        }


        public IActionResult Register()
        {
            if (_signinManager.IsSignedIn(User))
                return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(registerViewModel);
            }
            else
            {
                var user = _mapper.Map<IdentityUser>(registerViewModel);

                var result = await _userManager.CreateAsync(user, registerViewModel.Password);
                if (result.Succeeded)
                {
                    var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var emailMessage = Url.Action("ConfirmEmail", "Account",
                        new { username = user.UserName, token = emailConfirmationToken }, Request.Scheme);
                    await _messageSender.SendEmailAsync(registerViewModel.Email, "تایید ایمیل", emailMessage);

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var item in result.Errors)
                    {
                        ModelState.AddModelError("", item.Description);
                    }
                }
            }
            return View();
        }

        public async Task<IActionResult> Login(string returnUrl = null)
        {
            if (_signinManager.IsSignedIn(User))
                return RedirectToAction("Index", "Home");
            ViewBag.returnTo = returnUrl;

            var model = new LoginViewModel
            {
                ExternalLogins = await _signinManager.GetExternalAuthenticationSchemesAsync(),
                ReturnUrl = returnUrl
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (_signinManager.IsSignedIn(User))
                return RedirectToAction("Index", "Home");
            model.ExternalLogins = await _signinManager.GetExternalAuthenticationSchemesAsync();
            model.ReturnUrl = returnUrl;
            ViewBag.returnTo = returnUrl;

            if (ModelState.IsValid)
            {
                var result = await _signinManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, true);
                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Index", "Home");
                }
                if (result.IsLockedOut)
                {
                    ViewBag.error = "اکانت شما به دلیل تلاش های ناموفق بسته شده است.";
                    return View(model);
                }

                ModelState.AddModelError("", "نام کاربری یا رمز عبور اشتباه وارد شده است.");
            }

            return View(model);
        }

        public IActionResult ExtrtnalLogin(string provider, string returnUrl)
        {
            var redirectUrl = Url.Action("ExternalLoginCallBack", "Account", new { ReturnUrl = returnUrl });

            var properties = _signinManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        public async Task<IActionResult> ExternalLoginCallBack(string returnUrl = null, string remoteErrors = null)
        {
            if (string.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
            {
                returnUrl = Url.Content("~/");
            }

            var model = new LoginViewModel
            {
                ReturnUrl = returnUrl,
                ExternalLogins = await _signinManager.GetExternalAuthenticationSchemesAsync()
            };

            if(remoteErrors!=null)
            {
                ModelState.AddModelError("", $"Error : {remoteErrors}");
                return View("Login", model);
            }

            var externalLoginInfo = await _signinManager.GetExternalLoginInfoAsync();
            if(externalLoginInfo==null)
            {
                ModelState.AddModelError("", "LoadingExternalLoginInfo : مشکلی پیش آمد.");
                return View("Login", model);
            }

            var signinResult =await  _signinManager.ExternalLoginSignInAsync(externalLoginInfo.LoginProvider,
                externalLoginInfo.ProviderKey, true, true);

            if (signinResult.Succeeded)
            {
                return Redirect(returnUrl);
            }

            var email = externalLoginInfo.Principal.FindFirst(ClaimTypes.Email).Value;
            if(email!=null)
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    var username = email.Split('@')[0];
                    if (username.Length > 10)
                        username = username.Substring(0, 10);
                    user = new IdentityUser
                    {
                        UserName = username,
                        Email = email,
                        EmailConfirmed = true
                    };
                    await _userManager.CreateAsync(user);
                }

                await _userManager.AddLoginAsync(user, externalLoginInfo);
                await _signinManager.SignInAsync(user,true);

                return Redirect(returnUrl);
            }

            ViewBag.ErrorMessage = $"نمیتوان اطلاعاتی از {externalLoginInfo.LoginProvider} دریافت کرد .";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signinManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string username, string token)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(token))
                return NotFound();
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                return NotFound();

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return View();
            }
            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IsUsernameInUse(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                return Json(true);

            return Json("نام کاربری وارد شده در سایت موجود است.");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IsEmailInUse(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return Json(true);

            return Json("ایمیل وارد شده در سایت موجود است.");
        }
    }
}
