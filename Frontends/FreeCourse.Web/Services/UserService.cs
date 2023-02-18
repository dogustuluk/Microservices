using FreeCourse.Web.Models;
using FreeCourse.Web.Services.Interfaces;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace FreeCourse.Web.Services
{
    public class UserService : IUserService
    {
        private readonly HttpClient _client;

        public UserService(HttpClient client)
        {
            _client = client;
        }

        public async Task<UserViewModel> GetUser()
        {
            /*token ile ilgili
             * _client kullandık fakat token set etmedik. Her _client kullandığımızda token eklemiyoruz. Bir handler yazıyoruz ve herhangi bir istek yapıldığında o otomatik olarak araya girip bir access token ekleyecek. eğer access token geçersiz ise refresh token ekleyip yeniden gönderecek.
             */
            return await _client.GetFromJsonAsync<UserViewModel>("/api/user/getuser");//base uri startup tarafında UserService kısmından geliyor.
        }
    }
}
