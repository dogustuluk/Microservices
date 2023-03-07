using IdentityModel.Client;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
namespace FreeCourse.Gateway.DelegateHandlers
{
    /// <summary>
    /// kullanıcı giriş yaptığında metot araya girip eski token'ı alıyor ve identity server'a yolluyor. identity server ise fake payment api'sine ve discount api'sine izin veren yeni token'ı yollamış oluyor. Delege ilgili olan dışa kapalı mikroservislere istek yapıldığında çalışmak üzere config dosyasında yapılandırıldı.
    /// </summary>
    public class TokenExchangeDelegateHandler : DelegatingHandler
    {
        //identityServer'a yeni bir istek yapacağımız için
        private readonly HttpClient _httpClient;
        //Config dosyamızda clien id ve client secret okuyacağımız için
        private readonly IConfiguration _configuration;
        
        private string _accessToken;
        public TokenExchangeDelegateHandler(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        //token al
        private async Task<string> GetToken(string requestToken)
        {
            if (!string.IsNullOrEmpty(_accessToken))
            {
                return _accessToken;
            }
            //discovery'e bağlan
            var disco = await _httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = _configuration["IdentityServerURL"],
                Policy = new DiscoveryPolicy { RequireHttps = false }
            });
            if (disco.IsError)
            {
                throw disco.Exception;
            }

            TokenExchangeTokenRequest tokenExchangeTokenRequest = new TokenExchangeTokenRequest()
            {
                Address = disco.TokenEndpoint,
                ClientId = _configuration["ClientId"],
                ClientSecret = _configuration["ClientSecret"],
                GrantType = _configuration["TokenGrantType"],
                SubjectToken = requestToken,
                SubjectTokenType = "urn:ietf:params:oauth:token-type:access-token",
                Scope = "openid discount_fullpermission payment_fullpermission",
            };

            var tokenResponse = await _httpClient.RequestTokenExchangeTokenAsync(tokenExchangeTokenRequest);

            //hata kontrolü
            if (tokenResponse.IsError)
            {
                throw tokenResponse.Exception;
            }

            _accessToken = tokenResponse.AccessToken;
            return _accessToken;//yeni token alınmış oldu

            //şimdi yeni token'ı override edilen metotta ver.
        

        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            //yeni token'ı elde et.
            var requestToken = request.Headers.Authorization.Parameter;

            //yeni token'ı al.
            var newToken = await GetToken(requestToken);

            request.SetBearerToken(newToken);

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
