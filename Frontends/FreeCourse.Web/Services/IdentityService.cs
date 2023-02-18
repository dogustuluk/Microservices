using FreeCourse.Shared.Dtos;
using FreeCourse.Web.Models;
using FreeCourse.Web.Services.Interfaces;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;
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
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ClientSettings _clientSettings;
        private readonly ServiceApiSettings _serviceApiSettings;

        public IdentityService(HttpClient client, IHttpContextAccessor httpContextAccessor, IOptions<ClientSettings> clientSettings, IOptions<ServiceApiSettings> serviceApiSettings)
        {
            _httpClient = client;
            _httpContextAccessor = httpContextAccessor;
            _clientSettings = clientSettings.Value;
            _serviceApiSettings = serviceApiSettings.Value;
        }

        public async Task<TokenResponse> GetAccessTokenByRefreshToken()
        {
            //endpoint'i al
            var disco = await _httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = _serviceApiSettings.BaseUri,
                Policy = new DiscoveryPolicy { RequireHttps = false }
            });
            if (disco.IsError)
            {
                throw disco.Exception;
            }

            //cookie'den refresh token al.
            var refreshToken = await _httpContextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);
            //refreshtokenrequest oluştur
            RefreshTokenRequest refreshTokenRequest = new()
            {
                ClientId = _clientSettings.WebClientForUser.ClientId,
                ClientSecret = _clientSettings.WebClientForUser.ClientSecret,
                RefreshToken = refreshToken,
                Address = disco.TokenEndpoint
            };
            //yeni bir token almak için istek oluştur
            var token = await _httpClient.RequestRefreshTokenAsync(refreshTokenRequest);
            if (token.IsError)
            {
                return null; //hata da dönebiliriz.
                //log yapabiliriz.
            }
            //yeni değerleri kaydet
            var authenticationTokens = new List<AuthenticationToken>()
            {
                new AuthenticationToken{Name = OpenIdConnectParameterNames.AccessToken, Value = token.AccessToken},
                new AuthenticationToken{Name = OpenIdConnectParameterNames.RefreshToken, Value = token.RefreshToken},
                new AuthenticationToken{Name = OpenIdConnectParameterNames.ExpiresIn, Value = DateTime.Now.AddSeconds(token.ExpiresIn).ToString("o",CultureInfo.InvariantCulture)}
            };

            //elimizdeki cookie'yi set edicez.
            var authenticationResult = await _httpContextAccessor.HttpContext.AuthenticateAsync();//cookie'de bulunan property'leri alalım.
            var properties = authenticationResult.Properties;
            properties.StoreTokens(authenticationTokens); //sıfırdan oluşturmak yerine var olanın içerisine kaydetmiş oluyoruz.

            //cookie'ye kaydet
            await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, authenticationResult.Principal, properties);

            return token;
        }

        public async Task RevokeRefreshToken()
        {
            //endpoint'i al
            var disco = await _httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = _serviceApiSettings.BaseUri,
                Policy = new DiscoveryPolicy { RequireHttps = false }
            });
            if (disco.IsError)
            {
                throw disco.Exception;
            }

            //refresh token'ı al.Sadece refresh token revoke edilir.
            var refreshToken = await _httpContextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);
            //token revocation request oluştur
            TokenRevocationRequest tokenRevocationRequest = new()
            {
                ClientId = _clientSettings.WebClientForUser.ClientId,
                ClientSecret = _clientSettings.WebClientForUser.ClientSecret,
                Address = disco.RevocationEndpoint,
                Token = refreshToken,
                TokenTypeHint = "refresh_token" //bu property'mizde ise refresh token gönderdiğimizi belirtmek için kullanılır. isim ezber değildir
            };

            await _httpClient.RevokeTokenAsync(tokenRevocationRequest);
        }

        public async Task<Response<bool>> SignIn(SigninInput signinInput)
        {
            //token endpoint'ine git. Ardından http olarak ayarladık yapıyı fakat GetDiscoveryDocumentAsync isteği https olarak yapar bu durumu elle kapatmamız gerekmektedir. bunu Policy yazarak düzeltiriz.
            var disco = await _httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = _serviceApiSettings.BaseUri,
                Policy = new DiscoveryPolicy { RequireHttps = false }
            });
            if (disco.IsError)
            {
                throw disco.Exception;
            }
            //akış tipini yap. PasswordTokenRequest hazır bir sınıftır. içerisinde client_id,client_secret,grant_type,username ve password alır.
            var passwordTokenRequest = new PasswordTokenRequest
            {
                ClientId = _clientSettings.WebClientForUser.ClientId,
                ClientSecret = _clientSettings.WebClientForUser.ClientSecret,
                UserName = signinInput.Email,
                Password = signinInput.Password,
                Address = disco.TokenEndpoint //hangi adrese istek yapacağını bulur.
            };
            //istek yapmaya hazırız, gerekli bilgileri verdik.

            var token = await _httpClient.RequestPasswordTokenAsync(passwordTokenRequest);
            if (token.IsError)
            {
                //gelen token içerisindeki dataları oku.
                var responseContent = await token.HttpResponse.Content.ReadAsStringAsync();
                //type dönüşümü(deserialize) yap. PropertyNameCaseInsensitive ile property isimlerindeki büyük-küçük harf duyarlılığını kapatır.
                var errorDto = JsonSerializer.Deserialize<ErrorDto>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return Response<bool>.Fail(errorDto.Errors, 400);
            }
            //buraya gelirse token elimizdedir. access token ve/veya refresh token'a erişebiliriz.
            //burada token içerisinde kullanıcı bilgilerini bulundurmayız çünkü şişirmek istemeyiz token'ı. kullanıcı bilgilerini almak için user info endpoint'ine istek yapmalıyız.
            var userInfoRequest = new UserInfoRequest
            {
                //access token'ını ver
                Token = token.AccessToken,
                //istek yapacağı adresi ver
                Address = disco.UserInfoEndpoint,
            };
            //isteği gerçekleştir.
            var userInfo = await _httpClient.GetUserInfoAsync(userInfoRequest);
            if (userInfo.IsError)
            {
                throw userInfo.Exception;
            }
            //userinfo ile kullanıcıya ait bilgiler elde edilir. sub,name,role gibi bilgiler.
            //userinfo içerisindeki bilgileri cookie'ye göm çünkü cookie bazlı yetkilendirme ya da rol bazlı yetkilendirme(ya da başka bir yetki şekli) yapabilelim.
            //cookie oluştur.
            //kimlik oluştur.bu cookie claim nesnelerinden meydana gelir (key-value)
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(userInfo.Claims, CookieAuthenticationDefaults.AuthenticationScheme, "name", "role");

            //cookie'nin oluşması için bu sınıf gereklidir. Temelini belirler.
            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            //access token ve refresh token'ı da cookie üzerinde tutuyor olucaz ama bunu ayrı bir sınıf üzerinden tutuyoruz.
            var authenticationProperties = new AuthenticationProperties();
            authenticationProperties.StoreTokens(new List<AuthenticationToken>()
            {
                /*library desc
                 * Burada Microsoft.IdentityModel.Protocol.OpenIdConnect kullanmamızın sebebi identityserver'a yönlendirme yapmayıp kendi içimizde bir login işlemi yapmamızdan kaynakldır.
                 * Eğer identityServer'a yönlendirme yapıp bir login işlemi yapacaksak Microsoft.AspNetCore.Authentication.OpenIdConnect kütüphanesini yüklememiz gerekmektedir. 
                 */
                new AuthenticationToken { Name = OpenIdConnectParameterNames.AccessToken, Value = token.AccessToken },
                new AuthenticationToken { Name = OpenIdConnectParameterNames.RefreshToken, Value = token.RefreshToken },
                new AuthenticationToken { Name = OpenIdConnectParameterNames.ExpiresIn, Value = DateTime.Now.AddSeconds(token.ExpiresIn).ToString("o",CultureInfo.InvariantCulture) },
            });
            //access token, refresh token ve süreyi cookie içerisinde tutuyoruz.

            //beni hatırla durumunu kontrol et
            authenticationProperties.IsPersistent = signinInput.IsRemember;

            //cookie oluşmuş oldu
            await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,claimsPrincipal,authenticationProperties);

            return Response<bool>.Success(200);
        }
    }
}
