using FreeCourse.Services.Discount.Services;
using FreeCourse.Shared.ControllerBases;
using FreeCourse.Shared.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FreeCourse.Services.Discount.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiscountsController : CustomBaseController
    {
        /*userId
         * discount service'te userId istiyoruz. controller'da ise bu userId'yi client'tan beklemek yerine yani bu endpoint'i kullanan kullanıcıdan beklemek yerine jwt içerisinden alıyor olucaz. Bunu shared içerisinde tanıladığımız SharedIdentityService sınıfından alıyor olucaz. bu sınıf context üzerinden claim'lere gidip sub claim'i buluyor. sub claim'de de kullanıcı id'si vardır.
         */
        private readonly IDiscountService _discountService;
        private readonly ISharedIdentityService _sharedIdentityService;

        public DiscountsController(IDiscountService discountService, ISharedIdentityService sharedIdentityService)
        {
            _discountService = discountService;
            _sharedIdentityService = sharedIdentityService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return CreateActionResultInstance(await _discountService.GetAll());
        }
        
        [HttpGet("{id}")] //api/discount/5
        public async Task<IActionResult> GetById(int id)
        {
            var discount = await _discountService.GetById(id);
            return CreateActionResultInstance(discount);
        }

        [HttpGet]
        [Route("/api/[controller]/[action]/{code}")] //api/discount/GetByCode/kodalanı
        public async Task<IActionResult> GetByCode(string code)
        {
            var userId = _sharedIdentityService.GetUserId; //service'te de alabiliriz ama illa json içerisinde geçmek istemeyebiliriz bunu. eğer öyle istersek altta ayrı bir metot olarak tanımlayabiliriz. bu seçeneğin de olmasını istediğimizden dolayı burada alıyoruz userId'yi.
            var discount = await _discountService.GetByCodeAndUserId(code, userId);

            return CreateActionResultInstance(discount);
        }

        [HttpPost]
        public async Task<IActionResult> Save(Models.Discount discount)
        {
            return CreateActionResultInstance(await _discountService.Save(discount));
        }

        [HttpPut]
        public async Task<IActionResult> Update(Models.Discount discount)
        {
            return CreateActionResultInstance(await _discountService.Update(discount));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return CreateActionResultInstance(await _discountService.Delete(id));
        }

    }
}
