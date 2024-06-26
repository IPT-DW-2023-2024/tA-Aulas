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

using Aulas.Data;
using Aulas.Models;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Aulas.Areas.Identity.Pages.Account {
   public class RegisterModel : PageModel {
      private readonly SignInManager<IdentityUser> _signInManager;
      private readonly UserManager<IdentityUser> _userManager;
      private readonly IUserStore<IdentityUser> _userStore;
      private readonly IUserEmailStore<IdentityUser> _emailStore;
      private readonly ILogger<RegisterModel> _logger;
      private readonly IEmailSender _emailSender;

      private readonly ApplicationDbContext _context;

      public RegisterModel(
          UserManager<IdentityUser> userManager,
          IUserStore<IdentityUser> userStore,
          SignInManager<IdentityUser> signInManager,
          ILogger<RegisterModel> logger,
          IEmailSender emailSender,
          ApplicationDbContext context) {
         _userManager = userManager;
         _userStore = userStore;
         _emailStore = GetEmailStore();
         _signInManager = signInManager;
         _logger = logger;
         _emailSender = emailSender;
         _context = context;
      }

      /// <summary>
      ///    objeto que vai ser consumido
      ///    na interface gráfica desta página 
      /// </summary>
      [BindProperty]
      public InputModel Input { get; set; }

      /// <summary>
      ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
      ///     directly from your code. This API may change or be removed in future releases.
      /// </summary>
      public string ReturnUrl { get; set; }

      /// <summary>
      ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
      ///     directly from your code. This API may change or be removed in future releases.
      /// </summary>
      public IList<AuthenticationScheme> ExternalLogins { get; set; }




      /// <summary>
      /// classe que vai gerar o objeto a ser utilizado
      /// na interface gráfica desta página, 
      /// na interface que o consumidor vai consumir
      /// </summary>
      public class InputModel {
         /// <summary>
         ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
         ///     directly from your code. This API may change or be removed in future releases.
         /// </summary>
         [Required]
         [EmailAddress]
         [Display(Name = "Email")]
         public string Email { get; set; }

         /// <summary>
         ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
         ///     directly from your code. This API may change or be removed in future releases.
         /// </summary>
         [Required]
         [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
         [DataType(DataType.Password)]
         [Display(Name = "Password")]
         public string Password { get; set; }

         /// <summary>
         ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
         ///     directly from your code. This API may change or be removed in future releases.
         /// </summary>
         [DataType(DataType.Password)]
         [Display(Name = "Confirmar password")]
         [Compare("Password", ErrorMessage = "A password e a sua confirmação não correspondem.")]
         public string ConfirmPassword { get; set; }

         /// <summary>
         /// incorporação dos dados de um Professor
         /// no processo de registo de um novo utilizador
         /// </summary>
         public Professores Professor { get; set; }
      }


      public void OnGet(string returnUrl = null) {
         ReturnUrl = returnUrl;
         //   ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
      }

      /// <summary>
      /// Método que reage ao HTTP POST
      /// </summary>
      /// <param name="returnUrl"></param>
      /// <returns></returns>
      public async Task<IActionResult> OnPostAsync(string returnUrl = null) {
         returnUrl ??= Url.Content("~/");

         //   ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();



         if (ModelState.IsValid) {

            var user = CreateUser();

            await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
            await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

            // ação de, realmente, adicionar à BD (AspNetUsers) os dados do Utilizador
            var result = await _userManager.CreateAsync(user, Input.Password);

            if (result.Succeeded) {
               // houve sucesso na criação do Utilizador
               _logger.LogInformation("User created a new account with password.");

               // ###########################################
               // Associar este utilizador à Role Professor
               await _userManager.AddToRoleAsync(user, "Professor");
               // ###########################################

               try {
                  // ***********************************
                  // guardar os dados do Professor
                  // ***********************************

                  // criar uma ligação entre a tabela dos Utilizadores
                  // (neste caso, um Professor) e a tabela da Autenticação
                  Input.Professor.UserID = user.Id;

                  // adicionar os dados do Professor à BD
                  _context.Add(Input.Professor);
                  await _context.SaveChangesAsync();
                  // ***********************************
               }
               catch (Exception ex) {
                  // É NECESSÁRIO TRATAR A EXCEÇÃO
                  // DEFINIR A POLÍTICA, E AÇÕES, A EXECUTAR NESTA SITUAÇÃO
                  // POR EXEMPLO:
                  //    - apara o utilizador da tabela da Autenticação
                  //    - gerar mensagens de erro para a pessoa que está a criar o registo
                  //    - guardar os dados do Erro num LOG ou na BD
                  //    - etc.
                  throw;
               }


               // preparar os dados para o envio do email
               // ao utilizador para confirmar a conta criada
               var userId = await _userManager.GetUserIdAsync(user);
               var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
               code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
               var callbackUrl = Url.Page(
                   "/Account/ConfirmEmail",
                   pageHandler: null,
                   values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                   protocol: Request.Scheme);

               await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                   $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

               if (_userManager.Options.SignIn.RequireConfirmedAccount) {
                  return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
               }
               else {
                  await _signInManager.SignInAsync(user, isPersistent: false);
                  return LocalRedirect(returnUrl);
               }
            }
            foreach (var error in result.Errors) {
               ModelState.AddModelError(string.Empty, error.Description);
            }
         }

         // If we got this far, something failed, redisplay form
         return Page();
      }

      private IdentityUser CreateUser() {
         try {
            return Activator.CreateInstance<IdentityUser>();
         }
         catch {
            throw new InvalidOperationException($"Can't create an instance of '{nameof(IdentityUser)}'. " +
                $"Ensure that '{nameof(IdentityUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
         }
      }

      private IUserEmailStore<IdentityUser> GetEmailStore() {
         if (!_userManager.SupportsUserEmail) {
            throw new NotSupportedException("The default UI requires a user store with email support.");
         }
         return (IUserEmailStore<IdentityUser>)_userStore;
      }
   }
}
