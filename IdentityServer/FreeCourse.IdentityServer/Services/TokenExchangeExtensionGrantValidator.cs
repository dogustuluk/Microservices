using IdentityServer4.Validation;
using System.Linq;
using System.Threading.Tasks;

namespace FreeCourse.IdentityServer.Services
{
    public class TokenExchangeExtensionGrantValidator : IExtensionGrantValidator
    {
        //akış ismi ver. OAuth2.0'ın vermiş olduğu isimlendirme standardına göre yazılır.
        public string GrantType => "urn:ietf:params:oauth:grant-type:token-exchange";
        private readonly ITokenValidator _tokenValidator; //token doğrulama işlemi için gerekli.

        public TokenExchangeExtensionGrantValidator(ITokenValidator tokenValidator)
        {
            _tokenValidator = tokenValidator;
        }

        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            //request'i al.
            var requestRaw = context.Request.Raw.ToString();
            //request içerisinden token'ı al.
            var token = context.Request.Raw.Get("subject_token");

            if (string.IsNullOrEmpty(token))
            {
                context.Result = new GrantValidationResult(IdentityServer4.Models.TokenRequestErrors.InvalidRequest, "token missing");
                return;
            }

            //ardından token'ı doğrula
            var tokenValidateResult = await _tokenValidator.ValidateAccessTokenAsync(token);
            if (tokenValidateResult.IsError)
            {
                context.Result = new GrantValidationResult(IdentityServer4.Models.TokenRequestErrors.InvalidGrant, "token invalid");
                return;
            }

            //toke payload içerisinden kullanıcının id'sinin tutulduğu sub claim'ini al.
            var subjectClaim = tokenValidateResult.Claims.FirstOrDefault(c => c.Type == "sub");

            if (subjectClaim == null)
            {
                context.Result = new GrantValidationResult(IdentityServer4.Models.TokenRequestErrors.InvalidGrant, "token must contain sub value");
            }

            context.Result = new GrantValidationResult(subjectClaim.Value, "access_token", tokenValidateResult.Claims);

            return;
        }
    }
}
