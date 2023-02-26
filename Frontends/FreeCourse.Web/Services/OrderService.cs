using FreeCourse.Shared.Dtos;
using FreeCourse.Shared.Services;
using FreeCourse.Web.Models.FakePayments;
using FreeCourse.Web.Models.Orders;
using FreeCourse.Web.Services.Interfaces;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace FreeCourse.Web.Services
{
    public class OrderService : IOrderService
    {
        private readonly IPaymentService _paymentService;
        private readonly HttpClient _httpClient;
        private readonly IBasketService _basketService; //sepetteki dataları alacağımız için.
        private readonly ISharedIdentityService _sharedIdentityService;
        public OrderService(IPaymentService paymentService, HttpClient httpClient, IBasketService basketService, ISharedIdentityService sharedIdentityService)
        {
            _paymentService = paymentService;
            _httpClient = httpClient;
            _basketService = basketService;
            _sharedIdentityService = sharedIdentityService;
        }

        public async Task<OrderCreatedViewModel> CreateOrder(CheckoutInfoInput checkoutInfoInput)
        {
            /*models desc
             * order mikroservisinin application katmanındaki SaveOrder metodu bizden CreateOrderCommand bekliyor. Biz buradaki metotta buna karşılık gelen modelimizi oluşturmalıyız(OrderCreateInput)
             */
            //önce sepetteki dataları al
            var basket = await _basketService.Get();

            //ödeme oluştur
            var paymentInfoInput = new PaymentInfoInput()
            {
                CardName = checkoutInfoInput.CardName,
                CardNumber = checkoutInfoInput.CardNumber,
                CVV = checkoutInfoInput.CVV,
                Expiration = checkoutInfoInput.Expiration,
                TotalPrice = basket.TotalPrice
            };

            //ödeme işlemini gerçekleştir
            /*desc
             * Eğer paymentInfoInput ile bize bir id geliyorsa bu id'yi order mikroservisinin veri tabanında da ödemenin id'sini tutabiliriz.
             * CreateOrderCommand içerisinde PaymentNo tutulup ilgili ödemeyi ilgili siparişle eşleştirebiliriz.
             */
            var responsePayment = await _paymentService.ReceivePayment(paymentInfoInput);

            if (!responsePayment)
            {
                return new OrderCreatedViewModel() { Error = "Ödeme Alınamadı", IsSuccessful = false };
            }

            //sipariş oluştur. Öncesinde OrderCreateInput oluşturmak gereklidir.
            var orderCreateInput = new OrderCreateInput()
            {
                BuyerId = _sharedIdentityService.GetUserId,
                Address = new AddressCreateInput()
                {
                    Province = checkoutInfoInput.Province,
                    District = checkoutInfoInput.District,
                    Street = checkoutInfoInput.Street,
                    Line = checkoutInfoInput.Line,
                    ZipCode = checkoutInfoInput.ZipCode
                }
            };

            //orderItems'lar için sepetteki item'leri tek tek ekle
            basket.BasketItems.ForEach(x =>
            {
                var orderItem = new OrderItemCreateInput
                {
                    ProductId = x.CourseId,
                    Price = x.GetCurrentPrice,
                    PictureUrl = "",
                    ProductName = x.CourseName
                };
                orderCreateInput.OrderItems.Add(orderItem); //OrderCreateInput sınıfının ctor'unda OrderItems = new List<OrderItemCreateInput>(); kodu yazılmalıdır.
            });

            //gönderilecek data hazır.
            var response = await _httpClient.PostAsJsonAsync<OrderCreateInput>("orders", orderCreateInput);
            //response'dan ne döndüğüne bakmak için order mikroservisine gidip, application katmanında handlers içerisinde bulunan CreateOrderCommandHandler sınıfına bakmak gereklidir. (CreatedOrderDto döner ve içerisinde sadece OrderId'yi doldurur.) CreateOrderDto'ya karşılık gelen sınıfımız ise OrderCreateViewModel vardır.
            if (!response.IsSuccessStatusCode)
            {
                //response'u 5sn içerisinde tekrar gerçekleştirecek bir reTry mekanizması kur.
                //loglama yap
                return new OrderCreatedViewModel() { Error = "Sipariş Oluşturulamadı", IsSuccessful = false };
            }
            //ödeme gerçekleşti ve sipariş oluştu

            //var responseString = response.Content.ReadAsStringAsync(); //debug yapmak için yazdık.

            var orderCreatedViewModel = await response.Content.ReadFromJsonAsync<Response<OrderCreatedViewModel>>();

            orderCreatedViewModel.Data.IsSuccessful = true;//true'ya set ediyoruz çünkü OrderController içerisinde if ile bunun kontrolünü yapıyoruz.

            //ödeme işlemi gerçekleştikten sonra sepeti silmemiz gerekir.
            await _basketService.Delete();

            return orderCreatedViewModel.Data;
        }

        public async Task<List<OrderViewModel>> GetOrder()
        {
            /*response
             * geriye dönüş tipi için order mikroservisinde application katmanındaki handlers klasörünün içerisinde bulunan GetOrderByUserIdHandler sınıfına bak. (Response<List<OrderDto>>) olduğunu görücez.
             */
            var response = await _httpClient.GetFromJsonAsync<Response<List<OrderViewModel>>>("orders");

            return response.Data;
        }

        public async Task<OrderSuspendViewModel> SuspendOrder(CheckoutInfoInput checkoutInfoInput)
        {
            //FakePayment'a istek gerçekleştir.

            //basket'i al
            var basket = await _basketService.Get();

            //sipariş oluştur
            var orderCreateInput = new OrderCreateInput()
            {
                BuyerId = _sharedIdentityService.GetUserId,
                Address = new AddressCreateInput()
                {
                    Province = checkoutInfoInput.Province,
                    District = checkoutInfoInput.District,
                    Street = checkoutInfoInput.Street,
                    Line = checkoutInfoInput.Line,
                    ZipCode = checkoutInfoInput.ZipCode
                },
            };
            //sepetteki ürünleri al
            basket.BasketItems.ForEach(x =>
            {
                var orderItem = new OrderItemCreateInput()
                {
                    ProductId = x.CourseId,
                    ProductName = x.CourseName,
                    PictureUrl = "",
                    Price = x.GetCurrentPrice
                };
                orderCreateInput.OrderItems.Add(orderItem);
            });

            var paymentInfoInput = new PaymentInfoInput()
            {
                CardName = checkoutInfoInput.CardName,
                CardNumber = checkoutInfoInput.CardNumber,
                CVV = checkoutInfoInput.CVV,
                Expiration = checkoutInfoInput.Expiration,
                TotalPrice = basket.TotalPrice,
                Order = orderCreateInput

            };

            //payment'a istek yap
            var responsePayment = await _paymentService.ReceivePayment(paymentInfoInput);
            if (!responsePayment)
            {
                return new OrderSuspendViewModel() { Error = "Ödeme Alınamadı", IsSuccessful = false };
            }

            //sepeti sil
            await _basketService.Delete();

            return new OrderSuspendViewModel() { IsSuccessful = true };

        }

    }

}


