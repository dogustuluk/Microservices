using FreeCourse.Web.Models;
using FreeCourse.Web.Services.Interfaces;
using IdentityModel.AspNetCore.AccessTokenManagement;
using IdentityModel.Client;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Threading.Tasks;

namespace FreeCourse.Web.Services
{
    //IdentityModel.AspNetCore paketini yükle
    public class ClientCredentialTokenService : IClientCredentialTokenService
    {
        private readonly ServiceApiSettings _serviceApiSettings;
        private readonly ClientSettings _clientSettings;
        private readonly IClientAccessTokenCache _clientAccessTokenCache; //gelen token'ı cache'leyeceğimiz için gereklidir.
        private readonly HttpClient _httpClient;
        public ClientCredentialTokenService(IOptions<ServiceApiSettings> serviceApiSettings, IOptions<ClientSettings> clientSettings, IClientAccessTokenCache clientAccessTokenCache, HttpClient httpClient)
        {
            _serviceApiSettings = serviceApiSettings.Value;
            _clientSettings = clientSettings.Value;
            _clientAccessTokenCache = clientAccessTokenCache;
            _httpClient = httpClient;
        }

        public async Task<string> GetToken()
        {
            //cache'ten kontrol et
            var currentToken = await _clientAccessTokenCache.GetAsync("WebClientToken");
            //cachte varsa
            if (currentToken != null)
            {
                return currentToken.AccessToken;
            }

            //yoksa discovery endpoint'e git.
            var disco = await _httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = _serviceApiSettings.IdentityBaseUri,
                Policy = new DiscoveryPolicy { RequireHttps = false }
            });
            if (disco.IsError)
            {
                throw disco.Exception;
            }

            //disco varsa client credentials grant type'ını kullanarak istek yapabiliriz.
            var clientCredentialTokenRequest = new ClientCredentialsTokenRequest
            {
                ClientId = _clientSettings.WebClient.ClientId,
                ClientSecret = _clientSettings.WebClient.ClientSecret,
                Address = disco.TokenEndpoint
            };

            //token gelmesi için
            var newToken = await _httpClient.RequestClientCredentialsTokenAsync(clientCredentialTokenRequest);

            if (newToken.IsError)
            {
                throw newToken.Exception;
            }

            //elimizde token var, bunu cache'e kaydet.
            await _clientAccessTokenCache.SetAsync("WebClientToken", newToken.AccessToken, newToken.ExpiresIn);

            return newToken.AccessToken;
        }
    }
}
