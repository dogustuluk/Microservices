using FreeCourse.Shared.Dtos;
using FreeCourse.Web.Models;
using IdentityModel.Client;
using System.Threading.Tasks;

namespace FreeCourse.Web.Services.Interfaces
{
    public interface IIdentityService
    {
        /// <summary>
        /// Kullanıcının login olmasıyla ilgili email ve şifresini gönderecek ve identity server'a gidip token alacak.
        /// </summary>
        /// <returns></returns>
        Task<Response<bool>> SignIn(SignInInput signInInput);
        /// <summary>
        /// Access token'ın ömrü dolduğunda elimizdeki refresh token ile beraber cookie'den refresh token'ı okuyup yeni bir access token elde eder.
        /// </summary>
        /// <returns></returns>
        Task<TokenResponse> GetAccessTokenByRefreshToken(); //herhangi bir token sınıfı oluşturmadan, identity model kütüphanesinden TokenResponse alırız.
        /// <summary>
        /// Refresh token'ın geçerliliğini ortadan kaldırır.
        /// </summary>
        /// <returns></returns>
        Task RevokeRefreshToken();
    }
}
