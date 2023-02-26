using FreeCourse.Services.Order.Infrastructure;
using FreeCourse.Shared.Messages;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeCourse.Services.Order.Application.Consumers
{
    /*desc
     * Ortak namespace olduğu için direkt olarak kuyruktan dinliyor olacak
     */
    public class CreateOrderMessageCommandConsumer : IConsumer<CreateOrderMessageCommand>
    {
        //db ile ilgili işlem yapacağımız için db'yi alıyoruz.
        private readonly OrderDbContext _orderDbContext;

        public CreateOrderMessageCommandConsumer(OrderDbContext orderDbContext)
        {
            _orderDbContext = orderDbContext;
        }

        public async Task Consume(ConsumeContext<CreateOrderMessageCommand> context)
        {
            //context üzerinden rabbitMQ'daki mesaj gelecek.
            //önce bir adres oluştur
            var newAddress = new Domain.OrderAggregate.Address(context.Message.Address.Province, context.Message.Address.District, context.Message.Address.Street, context.Message.Address.ZipCode, context.Message.Address.Line);
            //order'ı veriyoruz.
            Domain.OrderAggregate.Order order = new Domain.OrderAggregate.Order(context.Message.BuyerId, newAddress);
            //order'a item'ları ekle
            context.Message.OrderItems.ForEach (x =>
            {
                order.AddOrderItem(x.ProductId, x.ProductName, x.Price, x.PictureUrl);
            });

            //ekle ve kaydetme işlemini yap
            await _orderDbContext.Orders.AddAsync(order);
            await _orderDbContext.SaveChangesAsync();
            //massTransit'e haberdar et
        }
    }
}
