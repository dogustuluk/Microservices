using FreeCourse.Shared.Dtos;
using FreeCourse.Web.Models.PhotoStocks;
using FreeCourse.Web.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace FreeCourse.Web.Services
{
    /*kaydetme
     * photosController'da sadece dosyanın ismini dönüyoruz. Klasör adını tutmuyoruz. Çünkü ilerleyen zamanlarda klasör adını değiştirmek istersek db'deki tüm klasör isimlerini de değiştirmek gereklidir.
     */
    public class PhotoStockService : IPhotoStockService
    {
        private readonly HttpClient _httpClient;

        public PhotoStockService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> DeletePhoto(string photoUrl)
        {
            var response = await _httpClient.DeleteAsync($"photos?photoUrl={photoUrl}");
            return response.IsSuccessStatusCode;
        }

        public async Task<PhotoViewModel> UploadPhoto(IFormFile photo)
        {
            if (photo == null || photo.Length <= 0)
            {
                return null;
            }

            //altta '.' koymamıza gerek yoktur. Path.GetExtension ile nokta ifadesi gelecektir.
            //örn isim -> 15616512305.jpg
            var randomFileName = $"{Guid.NewGuid().ToString()}{Path.GetExtension(photo.FileName)}";

            //Önce byte dizisi al çünkü PhotoStock Controller'a post işlemi yapıcaz.
            using var memoryStream = new MemoryStream();
            await photo.CopyToAsync(memoryStream);
            //request'in body'sine yolla
            var multipartContent = new MultipartFormDataContent();
            //ekle
            /*multipartContent desc
             * burada "photo" adını birebir vermemiz gerekmektedir çünkü PhotosController içerisinde bulunan PhotoSave metodunun parametrelerinde PhotoSave(IFormFile photo) şeklinde verdik. buradaki parametre adı ile eşleşmesi gerekmektedir.
             */
            multipartContent.Add(new ByteArrayContent(memoryStream.ToArray()), "photo", randomFileName);
            //
            var response = await _httpClient.PostAsync("photos", multipartContent);
            //cevap almamız gerekir
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            //photo url gelecek
            var responseSuccess = await response.Content.ReadFromJsonAsync<Response<PhotoViewModel>>();
            return responseSuccess.Data;
        }
    }
}
