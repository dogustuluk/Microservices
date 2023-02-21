using FreeCourse.Web.Models.Baskets;
using FreeCourse.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FreeCourse.Web.Controllers
{//her kullanıcının tek bir sepeti olacağı için isimlendirme yaparken çoğul kullanmadık. 
    [Authorize]
    public class BasketController : Controller
    {
        private readonly ICatalogService _catalogService;
        private readonly IBasketService _basketService;

        public BasketController(ICatalogService catalogService, IBasketService basketService)
        {
            _catalogService = catalogService;
            _basketService = basketService;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _basketService.Get());
        }
        public async Task<IActionResult> AddBasketItem(string courseId)
        {
            //önce kurs'u al
            var course = await _catalogService.GetByCourseId(courseId);
            //gelen course'tan basketItem al
            var basketItem = new BasketItemViewModel
            {
                CourseId = course.Id,
                CourseName = course.Name,
                Price = course.Price
            };

            await _basketService.AddBasketItem(basketItem);

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> RemoveBasketItem(string courseId)
        {
            await _basketService.RemoveBasketItem(courseId);
            //yukarıdaki kodun true-false'unu kontrol et. var result = await _basketService.RemoveBasketItem(courseId); diyip if ile kontrol edilebilir.
            return RedirectToAction(nameof(Index));
        }
    }
}
