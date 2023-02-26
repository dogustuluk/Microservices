using FreeCourse.Services.FakePayment.Models;
using FreeCourse.Shared.ControllerBases;
using FreeCourse.Shared.Dtos;
using FreeCourse.Shared.Messages;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace FreeCourse.Services.FakePayment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FakePaymentsController : CustomBaseController
    {
        private readonly ISendEndpointProvider _sendEndpointProvider;

        public FakePaymentsController(ISendEndpointProvider sendEndpointProvider)
        {
            _sendEndpointProvider = sendEndpointProvider;
        }

        [HttpPost]
        public async Task<IActionResult> ReceivePayment(PaymentDto paymentDto)
        {
            //paymentDto ile ödeme işlemi gerçekleştir.
            //mesaj gönderebilmemiz için ISend ile ilgili bir interface'ini al.
            var sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:create-order-service"));

            //Önce Shared içerisindeki Message klasörü altında bulunan CreateOrderMessageCommand'i gönder.
            var createOrderMessageCommand = new CreateOrderMessageCommand();
            /*nesne örneği alınan CreateOrderMessageCommand'i doldur
             * eğer birden fazla mapping işlemi olacaksa burada autoMapper ya da mapster gibi mapleme kütüphaneleri kullanılabilir.
             */
            createOrderMessageCommand.BuyerId = paymentDto.Order.BuyerId;
            createOrderMessageCommand.Address.Province = paymentDto.Order.Address.Province;
            createOrderMessageCommand.Address.District = paymentDto.Order.Address.District;
            createOrderMessageCommand.Address.Street = paymentDto.Order.Address.Street;
            createOrderMessageCommand.Address.Line = paymentDto.Order.Address.Line;
            createOrderMessageCommand.Address.ZipCode = paymentDto.Order.Address.ZipCode;
            //siparişle ilgili itemları al
            paymentDto.Order.OrderItems.ForEach(x =>
            {
                createOrderMessageCommand.OrderItems.Add(new OrderItem
                {
                    PictureUrl = x.PictureUrl,
                    Price= x.Price,
                    ProductId= x.ProductId,
                    ProductName = x.ProductName
                });
            });

            //elimizde mesaj var. Şimdi bu mesajı gönder
            await sendEndpoint.Send<CreateOrderMessageCommand>(createOrderMessageCommand);

            //limit yetersizliği dönülebilir, kredi kartı kapanması durumu, birden fazla kredi kartı kullanma gibi durumlar oluşturulabilir. Burada sadece 200 ile dönmüş olduk ama geliştirilebilir.
            return CreateActionResultInstance(Shared.Dtos.Response<NoContent>.Success(200));
        }
    }
}
