using FreeCourse.Web.Exceptions;
using FreeCourse.Web.Services.Interfaces;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace FreeCourse.Web.Handler
{
    public class ClientCredentialTokenHandler:DelegatingHandler
    {
        //token elde etmemiz için ClientCredentialTokenService'i alırız.
        private readonly IClientCredentialTokenService _clientCredentialTokenService;

        public ClientCredentialTokenHandler(IClientCredentialTokenService clientCredentialTokenService)
        {
            _clientCredentialTokenService = clientCredentialTokenService;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            //request'in header'ına ekle
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", await _clientCredentialTokenService.GetToken());

            //401 hatası gelebilir, kontrolünü yap
            var response = await base.SendAsync(request, cancellationToken);
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new UnAuthorizeException();
            }
            return response;
        }
    }
}
