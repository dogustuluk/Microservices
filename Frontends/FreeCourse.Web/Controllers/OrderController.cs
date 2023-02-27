using FreeCourse.Web.Models.Orders;
using FreeCourse.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace FreeCourse.Web.Controllers
{
    public class OrderController : Controller
    {
        private readonly IBasketService _basketService;
        private readonly IOrderService _orderService;

        public OrderController(IBasketService basketService, IOrderService orderService)
        {
            _basketService = basketService;
            _orderService = orderService;
        }

        public async Task<IActionResult> Checkout()
        {
            //basket cart'ı al
            var basket = await _basketService.Get();
            ViewBag.basket = basket;

            return View(new CheckoutInfoInput());
        }
        [HttpPost]
        public async Task<IActionResult> Checkout(CheckoutInfoInput checkoutInfoInput)
        {
            /*ilk yol
             * var orderStatus = await _orderService.CreateOrder(checkoutInfoInput);
            if (!orderStatus.IsSuccessful)
            {
                TempData["error"] = orderStatus.Error;
                return RedirectToAction(nameof(Checkout));
            }
             */

            //1.yol senkron iletişim
            //var orderStatus = await _orderService.CreateOrder(checkoutInfoInput);

            //2.yol asenkron iletişim
            var orderSuspend = await _orderService.SuspendOrder(checkoutInfoInput);
            if (!orderSuspend.IsSuccessful)
            {
                var basket = await _basketService.Get();
                ViewBag.basket = basket;
                ViewBag.error = orderSuspend.Error;
                return View();
            }

            //1.yol senkron iletişim.
            //return RedirectToAction(nameof(SuccessfullCheckout), new { orderId = orderStatus.OrderId });

            //2.yol asenkron iletişim
            /*Random Id
             * SuccessfullCheckout metodunda orderId atadığımız için burada bir orderId vermemiz gerekir. Fakat orderSuspend üzerinden bir orderId gelmemekte. Burada random bir orderId atarak bir çözüm geliştirdik. Eğer bunun önüne geçmek istersek başta bir int değer geçmemiz yeterliydi. Yani FakePayment mikroservisi içerisinde bulunan FakePaymentsController'da  Response'a NoContent geçtik. Burada NoContent yerine bir dto dönseydik daha net bir çözüm geliştirmiş olurduk. Burada ödemenin id'sini bulabilirdik. Bu şekilde yapsaydık eğer ödeme no ile sipariş no'yu birbirinden ayırmış olurduk.
             */
            return RedirectToAction(nameof(SuccessfullCheckout), new { orderId = new Random().Next(1, 1000) });
        }
        public IActionResult SuccessfullCheckout(int orderId)
        {
            ViewBag.orderId = orderId;
            return View();

        }
        public async Task<IActionResult> CheckoutHistory()
        {
            return View(await _orderService.GetOrder());
        }
    }
}
