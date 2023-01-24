using FreeCourse.Services.Catalog.Dtos;
using FreeCourse.Services.Catalog.Services;
using FreeCourse.Shared.ControllerBases;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FreeCourse.Services.Catalog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : CustomBaseController
    {
        private readonly ICourseService _courseService;

        public CoursesController(ICourseService courseService)
        {
            _courseService = courseService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = await _courseService.GetAllAsync();
            return CreateActionResultInstance(response);
        }

        [HttpGet("{id}")] //courses/2 şeklinde url yapısı
        public async Task<IActionResult> GetById(string id)
        {
            var response = await _courseService.GetByIdAsync(id);

            return CreateActionResultInstance(response);
            /*CreateActionResultInstance
             * burada generic vermemize gerek yok çünkü base sınıfta parametre olarak generic geçiyoruz yani buraya bir nevi generic verilmiş hali geliyor. bu kod ile if-else kontrolleri ile response'un 200,204,404 gibi kodlarla mı geldiğini kontrol etmemiş oluruz.*/

        }

        [HttpGet]
        [Route("/api/[controller]/GetAllByUserId/{userId}")] //api/courses/getallbyuserid/5 url yapısı
        public async Task<IActionResult> GetAllByUserId(string userId)
        {
            var response = await _courseService.GetAllByUserIdAsync(userId);

            return CreateActionResultInstance(response);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CourseCreateDto courseCreateDto)
        {
            var response = await _courseService.CreateAsync(courseCreateDto);
            return CreateActionResultInstance(response);
        }
        [HttpPut]
        public async Task<IActionResult> Update(CourseUpdateDto courseUpdateDto)
        {
            var response = await _courseService.UpdateAsync(courseUpdateDto);
            return CreateActionResultInstance(response);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(string id)
        {
            var response = await _courseService.DeleteAsync(id);
            return CreateActionResultInstance(response);
        }
    }
}
