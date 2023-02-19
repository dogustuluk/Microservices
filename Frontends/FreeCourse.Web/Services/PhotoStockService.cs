using FreeCourse.Web.Models.PhotoStocks;
using FreeCourse.Web.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Threading.Tasks;

namespace FreeCourse.Web.Services
{
    public class PhotoStockService : IPhotoStockService
    {
        private readonly HttpClient _httpClient;
        public Task<bool> DeletePhoto(string photoUrl)
        {
            throw new System.NotImplementedException();
        }

        public Task<PhotoViewModel> UploadPhoto(IFormFile photo)
        {
            throw new System.NotImplementedException();
        }
    }
}
