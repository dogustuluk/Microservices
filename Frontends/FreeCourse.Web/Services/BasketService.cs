using FreeCourse.Shared.Dtos;
using FreeCourse.Web.Models.Baskets;
using FreeCourse.Web.Services.Interfaces;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace FreeCourse.Web.Services
{
    /*metotlar
     * Buradaki AddBasketItem ve RemoveBasketItem metotlarım BasketsController'da yok. Bu metotlar bize kolaylık sağlar. Bu metotlar içerisinde gerekli gördüğümüz implementasyon kodlarını kendimiz yazdık.
     * Discount metotlarını henüz kodlamadık çünkü daha discount microservice'i ile haberleşme sağlamadık. İlerleyen zamanlarda eklenecektir.
     */
    public class BasketService : IBasketService
    {
        private readonly HttpClient _httpClient;
        private readonly IDiscountService _discountService;
        public BasketService(HttpClient httpClient, IDiscountService discountService)
        {
            _httpClient = httpClient;
            _discountService = discountService;
        }

        public async Task AddBasketItem(BasketItemViewModel basketItemViewModel)
        {
            //basket'i  al
            var basket = await Get();

            if (basket != null)
            {
                //eklenen item daha önce yoksa
                if (!basket.BasketItems.Any(x => x.CourseId == basketItemViewModel.CourseId))
                {
                    basket.BasketItems.Add(basketItemViewModel);
                }
            }
            //sepette ilgili basket oluşmadıysa
            else
            {
                basket = new BasketViewModel(); //içerisinde userId belirtmemize gerek yok çünkü basket microservice'inde(BasketsController'da) shared identity service'ten çekiyoruz userId'yi.
                //
                basket.BasketItems.Add(basketItemViewModel);
            }
            //save or update yap
            await SaveOrUpdate(basket);
        }

        public async Task<bool> ApplyDiscount(string discountCode)
        {
            //önce ilk indirim kuponunu iptal et
            await CancelApplyDiscount();
            //basket'i al
            var basket = await Get();
            if (basket == null || basket.DiscountCode == null)
            {
                return false;
            }
            //önce indirimi al
            var hasDiscount = await _discountService.GetDiscount(discountCode);
            if (hasDiscount == null)
            {
                return false;
            }
            //hasDiscount varsa
            basket.DiscountRate = hasDiscount.Rate;
            basket.DiscountCode = hasDiscount.Code;

            await SaveOrUpdate(basket);
            return true;
        }

        public async Task<bool> CancelApplyDiscount()
        {
            //önce basket'i al.basket'in içerisinde iptal edicez. 
            var basket = await Get();
            if (basket != null || basket.DiscountCode == null)
            {
                return false;
            }

            //mevcut olan indirim kodunu null'a set et
            basket.DiscountCode = null;
            //basket'i tekrar update et.
            await SaveOrUpdate(basket);
            return true;

        }

        public async Task<bool> Delete()
        {
            var result = await _httpClient.DeleteAsync("baskets");

            return result.IsSuccessStatusCode;
        }

        public async Task<BasketViewModel> Get()
        {
            //mevcut datayı al
            var response = await _httpClient.GetAsync("baskets");

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            //BasketsController'daki Get metodundan BasketDto gelir. Bunu BasketViewModel'a dönüştürmemiz gereklidir.
            var basketViewModel = await response.Content.ReadFromJsonAsync<Response<BasketViewModel>>();

            return basketViewModel.Data;

            //discount tanımla
        }

        public async Task<bool> RemoveBasketItem(string courseId)
        {
            //basket'i al
            var basket = await Get();

            if (basket == null)
            {
                return false;
            }

            var deleteBasketItem = basket.BasketItems.FirstOrDefault(x => x.CourseId == courseId);
            if (deleteBasketItem == null)
            {
                return false;
            }

            var deleteResult = basket.BasketItems.Remove(deleteBasketItem);

            if (!deleteResult)
            {
                return false;
            }

            //sepette son ürünü sildiyse indirimi null'a çek
            if (!basket.BasketItems.Any())
            {
                basket.DiscountCode = null;
            }

            return await SaveOrUpdate(basket);
        }

        public async Task<bool> SaveOrUpdate(BasketViewModel basketViewModel)
        {
            /*userId desc
             * Sadece userId'yi set edip gönder.
             * Basket microservice'i içerisinde BasketController'ın constructor'ında biz userId için gerekli olan sharedIdentity'i aldık, burada tekrar almıyoruz.
             */
            var response = await _httpClient.PostAsJsonAsync<BasketViewModel>("baskets", basketViewModel);

            return response.IsSuccessStatusCode;


        }
    }
}
