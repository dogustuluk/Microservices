using FreeCourse.Services.Order.Domain.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeCourse.Services.Order.Domain.OrderAggregate
{
    //Bu sınıfı bir aggregate root kullandığı için başka bir aggregate root bu sınıfı kullanamaz.
    public class OrderItem:Entity
    {
        public string ProductId { get; private set; }
        public string ProductName { get; private set; }
        public string PictureUrl { get; private set; }
        public Decimal Price { get; private set; }
        //navigation property'sini eklemiyoruz çünkü bunun tek başına eklenmesini istemiyoruz. Bunu Order ekleyebilir çünkü Aggregateroot'umuzdur.
        public OrderItem()
        {

        }
        public OrderItem(string productId, string productName, string pictureUrl, decimal price)
        {
            ProductId = productId;
            ProductName = productName;
            PictureUrl = pictureUrl;
            Price = price;
        }

        //property'ler private set olduğu için dışarıdan set edilemezler. burada benim metotlarımı kullanarak belirttiğim business kuralları çerçevesinde set edebilirler.
        public void UpdateOrderItem(string productName, string pictureUrl, decimal price)
        {
            ProductName = productName;
            PictureUrl = pictureUrl;
            Price = price;
        }
    }
}
