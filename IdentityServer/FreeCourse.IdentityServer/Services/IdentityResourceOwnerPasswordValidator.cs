using FreeCourse.IdentityServer.Models;
using IdentityModel;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FreeCourse.IdentityServer.Services
{
    public class IdentityResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public IdentityResourceOwnerPasswordValidator(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            //email kontrolü
            //username olarak email gönderiyor olacağız.
            var existUser = await _userManager.FindByEmailAsync(context.UserName);

            if (existUser == null)
            {
                /*desc
                 * identity server'ın döneceği response'a ek yapmak istiyoruz.
                 */
                var errors = new Dictionary<string, object>();
                errors.Add("errors", new List<string>
                {
                    "E-mail veya şifreniz yanlış."
                });
                context.Result.CustomResponse = errors;
                return;
            }

            //şifre kontrolü
            var passwordCheck = await _userManager.CheckPasswordAsync(existUser, context.Password);
            if (passwordCheck == false)
            {
                var errors = new Dictionary<string, object>();
                errors.Add("errors", new List<string>
                {
                    "E-mail veya şifreniz yanlış."
                });
                context.Result.CustomResponse = errors;
                return;
            }

            //resource owner client credentials'ın kısaltılmış ismi ------------>>>>>>>>>> Password
            //token üretir.
            context.Result = new GrantValidationResult(existUser.Id.ToString(), OidcConstants.AuthenticationMethods.Password);
        }
    }
}
