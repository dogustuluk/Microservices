using FreeCourse.Web.Models.Orders;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FreeCourse.Web.Services.Interfaces
{
    /*senkron haberleşme
     * Eğer birden fazla mikroserviste senkron bir haberleşme yapmak istersek burada --> SAGA <-- gibi patternleri kullanmamız gerekir. Çünkü birden fazla mikroservice ile haberleşme durumunda distributed transaction'ı yönetmemiz gerekmektedir.
     * Distributed Transaction --->>> Yani; 3 tane mikroservisle işlem yapıyoruz veri tabanlarında işlem yapmak için, eğer bir tanesinde problem olursa kalan 2 tanede iptal edecek yeni istekler göndermemiz gerekir.
     */
    /* reTry mekanizması
     * Eğer ödeme işleminde bir hata meydana gelirse bir tane reTry mekanizması kurmamız gerekir. Bu mekanizma ile ödeme esnasında bir hata olursa ödeme yapan isteğin birkaç saniye içerisinde tekrardan oluşturulmasıdır. Detaylarını araştır.
     */
    public interface IOrderService
    {
        /// <summary>
        /// Senkron iletişim - direkt order mikroservisine istek yapılacak.
        /// </summary>
        /// <param name="checkoutInfoInput"></param>
        /// <returns></returns>
        Task<OrderCreatedViewModel> CreateOrder(CheckoutInfoInput checkoutInfoInput);//kurslar yani ürünler sepetten gelecek.
        
        /// <summary>
        /// Asenkron iletişim - sipariş bilgileri rabbitMQ'ya gönderilecek.
        /// </summary>
        /// <param name="checkoutInfoInput"></param>
        /// <returns></returns>
        Task<OrderSuspendViewModel> SuspendOrder(CheckoutInfoInput checkoutInfoInput);

        Task<List<OrderViewModel>> GetOrder();
    }
}
