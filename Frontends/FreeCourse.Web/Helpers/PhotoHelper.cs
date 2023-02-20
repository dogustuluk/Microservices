using FreeCourse.Web.Models;
using Microsoft.Extensions.Options;

namespace FreeCourse.Web.Helpers
{
    //url -> http:localhost//5012/photos/abc.jpg şeklinde gösterimini sağlamak için yazılmıştır.
    public class PhotoHelper
    {
        private readonly ServiceApiSettings _serviceApiSettings; //singleton yapıp alabiliriz.

        public PhotoHelper(IOptions<ServiceApiSettings> serviceApiSettings)
        {
            _serviceApiSettings = serviceApiSettings.Value;
        }

        public string GetPhotoStockUrl(string photoUrl)
        {
            return $"{_serviceApiSettings.PhotoStockUri}/photos/{photoUrl}";
        }

    }
}
