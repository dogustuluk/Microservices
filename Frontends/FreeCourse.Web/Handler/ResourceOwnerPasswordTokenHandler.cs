using FreeCourse.Web.Exceptions;
using FreeCourse.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace FreeCourse.Web.Handler
{
    public class ResourceOwnerPasswordTokenHandler:DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor; //cookieden okumak için.
        private readonly IIdentityService _identityService; //refresh token elde etmek için.
        private readonly ILogger<ResourceOwnerPasswordTokenHandler> _logger;

        public ResourceOwnerPasswordTokenHandler(IHttpContextAccessor httpContextAccessor, IIdentityService identityService, ILogger<ResourceOwnerPasswordTokenHandler> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _identityService = identityService;
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            //her bir istek başladığında SendAsync araya girip başlayacak.
            
            //önce access token'ı al.
            var accessToken = await _httpContextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);

            //gelen accessToken'ı metot içerisindeki request'in header'ına ekle
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);//spa olsaydı interceptor üzerinden ekleyecektik.
            
            //istek sonucunu takip et.
            var response = await base.SendAsync(request, cancellationToken);
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                //unauthorize ise refresh token ile yeni bir access token al.
                var tokenResponse = await _identityService.GetAccessTokenByRefreshToken();

                if (tokenResponse != null)
                {
                    //null değilse access token'ı tekrar header'a yolla
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);
                    response = await base.SendAsync(request, cancellationToken);
                }
            }
            //eğer buraya hala 401 unauthorized geliyorsa refresh token geçersizdir.
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                /*desc
                 * hata fırlat ve kullanıcıyı login ekranına yönlendir. Ama delegate üzerinden herhangi bir action sınıfına redirect yapamayız. hata fırlatırız sadece ve bu hatayı yakalayıp kullanıcıyı login ekranına göndeririz. burada uygulama geneline hata yakalayan bir middleware yazarız ve burada yakalama işlemini yapıp login ekranına yönlendirmeyi yaparız.
                 */
                throw new UnAuthorizeException(); //içerisine mesaj yazmaya gerek yok çünkü bunu globalde uygulama ayağa kalkarken uygulamada herhangi bir hata fırlatıldığında bizim yakalayacağımız hazır bir middleware'imiz olacak. İlgili middleware'de hatanın tipi UnAuthorizeException ise kullanıcıyı login ekranına döndürmüş olucaz.

            }

            return response;
        }
    }
}
