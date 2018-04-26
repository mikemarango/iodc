using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OIDC.IdentityServer.Services.Repository;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OIDC.IdentityServer.Controllers.UserRegistration
{
    public class UserRegistrationController : Controller
    {
        public UserRegistrationController(IUserRepository userRepository,
            IIdentityServerInteractionService interactionService)
        {
            UserRepository = userRepository;
            InteractionService = interactionService;
        }

        public IUserRepository UserRepository { get; }
        public IIdentityServerInteractionService InteractionService { get; }

        // GET: /<controller>/
        [HttpGet]
        public async Task<ActionResult> RegisterUser(string returnUrl)
        {
            var registerUserViewModel = new RegisterUserViewModel()
            {
                ReturnUrl = returnUrl
            };

            return await Task.FromResult(View(registerUserViewModel));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterUser(RegisterUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                // create user + claims
                var newUser = new Entities.User
                {
                    Password = model.Password,
                    Username = model.Username,
                    IsActive = true
                };
                newUser.Claims.Add(new Entities.Claim("country", model.Country));
                newUser.Claims.Add(new Entities.Claim("address", model.Address));
                newUser.Claims.Add(new Entities.Claim("given_name", model.Firstname));
                newUser.Claims.Add(new Entities.Claim("family_name", model.Lastname));
                newUser.Claims.Add(new Entities.Claim("email", model.Email));
                newUser.Claims.Add(new Entities.Claim("subscriptionlevel", "FreeUser"));

                if (model.IsExternalProvider)
                {
                    newUser.Logins.Add(new Entities.Login
                    {
                        LoginProvider = model.Provider,
                        ProviderKey = model.ProviderUserId
                    });
                }
                // add it through the repository
                await UserRepository.AddUser(newUser);

                if (!await UserRepository.SaveAsync())
                {
                    throw new Exception($"Creating a user failed.");
                }

                if (!model.IsExternalProvider)
                {
                    // log the user in
                    await HttpContext.SignInAsync(newUser.SubjectId, newUser.Username);
                }
                // continue with the flow     
                if (InteractionService.IsValidReturnUrl(model.ReturnUrl) || Url.IsLocalUrl(model.ReturnUrl))
                {
                    return Redirect(model.ReturnUrl);
                }

                return Redirect("~/");
            }

            // ModelState invalid, return the view with the passed-in model
            // so changes can be made
            return View(model);
        }
    }
}
