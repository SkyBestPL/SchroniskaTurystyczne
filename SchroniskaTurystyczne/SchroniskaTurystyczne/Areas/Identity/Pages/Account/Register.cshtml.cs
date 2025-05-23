﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using SchroniskaTurystyczne.Models;

namespace SchroniskaTurystyczne.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IUserStore<AppUser> _userStore;
        private readonly IUserEmailStore<AppUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterModel(
            UserManager<AppUser> userManager,
            IUserStore<AppUser> userStore,
            SignInManager<AppUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }
        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Imię jest wymagane.")]
            [StringLength(50, ErrorMessage = "Imię musi mieć przynajmniej {2} i maksymalnie {1} znaków.", MinimumLength = 2)]
            [Display(Name = "Imię")]
            public string FirstName { get; set; }

            [Required(ErrorMessage = "Nazwisko jest wymagane.")]
            [StringLength(50, ErrorMessage = "Nazwisko musi mieć przynajmniej {2} i maksymalnie {1} znaków.", MinimumLength = 2)]
            [Display(Name = "Nazwisko")]
            public string LastName { get; set; }
            [Required(ErrorMessage = "Email jest wymagany.")]
            [EmailAddress(ErrorMessage = "Niepoprawna struktura adresu email.")]
            [StringLength(100, ErrorMessage = "Email może mieć maksymalnie {1} znaków.")]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Hasło jest wymagane.")]
            [StringLength(20, ErrorMessage = "Hasło musi mieć przynajmniej {2} i maksymalnie {1} znaków.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Hasło")]
            public string Password { get; set; }

            [Required(ErrorMessage = "Powtórz hasło.")]
            [DataType(DataType.Password)]
            [Display(Name = "Potwierdź hasło")]
            [Compare("Password", ErrorMessage = "Hasła są różne.")]
            public string ConfirmPassword { get; set; }

            [Required(ErrorMessage = "Numer telefonu jest wymagany.")]
            [Phone]
            [Display(Name = "Numer telefonu")]
            [StringLength(9, ErrorMessage = "Telefon musi mieć 9 znaków.", MinimumLength = 9)]
            [DataType(DataType.PhoneNumber)]
            public string PhoneNumber { get; set; }

            [Required(ErrorMessage = "Wybierz swoją rolę.")]
            [Display(Name = "Rola")]
            public string Role { get; set; }
        }


        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = CreateUser();

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

                user.FirstName = Input.FirstName;
                user.LastName = Input.LastName;
                user.PhoneNumber = Input.PhoneNumber;

                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    if (Input.Role == "Guest" || Input.Role == "Exhibitor")
                    {
                        await _userManager.AddToRoleAsync(user, Input.Role);
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Nieprawidłowa rola.");
                        return Page();
                    }

                    var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var emailConfirmationResult = await _userManager.ConfirmEmailAsync(user, emailConfirmationToken);

                    if (!emailConfirmationResult.Succeeded)
                    {
                        foreach (var error in emailConfirmationResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        return Page();
                    }

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return Page();
        }

        private AppUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<AppUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(IdentityUser)}'. " +
                    $"Ensure that '{nameof(IdentityUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<AppUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<AppUser>)_userStore;
        }
    }
}
