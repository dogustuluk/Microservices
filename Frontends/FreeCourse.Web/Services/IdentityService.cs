using FreeCourse.Shared.Dtos;
using FreeCourse.Web.Models;
using FreeCourse.Web.Services.Interfaces;
using IdentityModel.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Threading.Tasks;

namespace FreeCourse.Web.Services
{
    public class IdentityService : IIdentityService
    {
        /*desc
         * identity service'e istek yapacağımız için http client nesnemizi almamız gerekmektedir.
         * httpcontext accessor almalıyız. çünkü cookie'ye erişimde bulunacağız.
         * clientId ve clientSecret'a ihtiyacımız var. ayrıca appsettings'ten data okumamız gerekmektedir. ClientSettings ve ServiceApiSettings. IOptions ile alırız constructor içerisinde. Bunu startup tarafında singleton olarak da ayarlayabilirdik.
         */
        private readonly HttpClient _client;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ClientSettings _clientSettings;
        private readonly ServiceApiSettings _serviceApiSettings;

        public IdentityService(HttpClient client, IHttpContextAccessor httpContextAccessor, IOptions<ClientSettings> clientSettings, IOptions<ServiceApiSettings> serviceApiSettings)
        {
            _client = client;
            _httpContextAccessor = httpContextAccessor;
            _clientSettings = clientSettings.Value;
            _serviceApiSettings = serviceApiSettings.Value;
        }

        public Task<TokenResponse> GetAccessTokenByRefreshToken()
        {
            throw new System.NotImplementedException();
        }

        public Task RevokeRefreshToken()
        {
            throw new System.NotImplementedException();
        }

        public Task<Response<bool>> SignIn(SignInInput signInInput)
        {
            throw new System.NotImplementedException();
        }
    }
}
