using FreeCourse.Services.FakePayment.Models;
using FreeCourse.Shared.ControllerBases;
using FreeCourse.Shared.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FreeCourse.Services.FakePayment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FakePaymentsController : CustomBaseController
    {
        [HttpPost]
        public IActionResult ReceivePayment(PaymentDto paymentDto)
        {
            //paymentDto ile ödeme işlemi gerçekleştir.
            //limit yetersizliği dönülebilir, kredi kartı kapanması durumu, birden fazla kredi kartı kullanma gibi durumlar oluşturulabilir. Burada sadece 200 ile dönmüş olduk ama geliştirilebilir.
            return CreateActionResultInstance(Response<NoContent>.Success(200));
        }
    }
}
