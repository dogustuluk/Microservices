using FreeCourse.Shared.Services;
using FreeCourse.Web.Models.Catalog;
using FreeCourse.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace FreeCourse.Web.Controllers
{
    [Authorize]
    public class CoursesController : Controller
    {
        private readonly ICatalogService _catalogService;
        private readonly ISharedIdentityService _sharedIdentityService;

        public CoursesController(ICatalogService catalogService, ISharedIdentityService sharedIdentityService)
        {
            _catalogService = catalogService;
            _sharedIdentityService = sharedIdentityService;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _catalogService.GetAllCourseByUserIdAsync(_sharedIdentityService.GetUserId));
        }
        public async Task<IActionResult> Create()
        {
            //category'leri al
            var categories = await _catalogService.GetAllCategoryAsync();
            //categories'den gelen dataları -> select list'e dönüştür.
            ViewBag.categoryList = new SelectList(categories, "Id", "Name");//kullanıcıya Name'i gözükür fakat arka tarafta Id'sini tutarız.
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CourseCreateInput courseCreateInput)
        {
            var categories = await _catalogService.GetAllCategoryAsync();
            ViewBag.categoryList = new SelectList(categories, "Id", "Name");
            if (!ModelState.IsValid)
            {
                return View();
            }
            //ViewModel içerisindeki UserId'yi doldur.
            courseCreateInput.UserId = _sharedIdentityService.GetUserId;

            await _catalogService.CreateCourseAsync(courseCreateInput);
            //if (result == false)
            //{
            //    //hata mesajı ver
            //    //loglama yap
            //}

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Update(string id)
        {
            //önce ilgili id'ye sahip course'u bul
            var course = await _catalogService.GetByCourseId(id);
            //category alınmalı çünkü kullanıcı kategoriyi değiştirebilir.
            var categories = await _catalogService.GetAllCategoryAsync();

            if (course == null)
            {
                //sayfaya yönlendirme yapılabilir, loglama olabilir, alert çıkartılabilir.
                RedirectToAction(nameof(Index));
            }

            ViewBag.categoryList = new SelectList(categories, "Id", "Name",course.Id);//seçilen kursu al
            //gelen course'u doldur.
            CourseUpdateInput courseUpdateInput = new()
            {
                Id = course.Id,
                Name = course.Name,
                Description = course.Description,
                Price = course.Price,
               // Feature = new FeatureViewModel { Duration = course.Feature.Duration }
               Feature = course.Feature,
               CategoryId = course.CategoryId,
               UserId = course.UserId,
               Picture = course.Picture
                
            };
            return View(courseUpdateInput);

        }
        [HttpPost]
        public async Task<IActionResult> Update(CourseUpdateInput courseUpdateInput)
        {
            var categories = await _catalogService.GetAllCategoryAsync();
            ViewBag.categoryList = new SelectList(categories, "Id", "Name", courseUpdateInput.Id);

            if (!ModelState.IsValid)
            {
                return View();
            }
            var response = await _catalogService.UpdateCourseAsync(courseUpdateInput);
            if (response == false)
            {
                //hata ver, loglama yap....
            }
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(string id)
        {
            await _catalogService.DeleteCourseAsync(id);
            //true-false durumunu da yap
            return RedirectToAction(nameof(Index));
        }
    }
}
