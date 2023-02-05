using FreeCourse.Services.Order.Domain.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeCourse.Services.Order.Domain.OrderAggregate
{
    /*EF Core features
     * owned types --> db'de bir karşılığı olmayacağı için bu tipe sahip sınıfları dbcontext içerisinde dbset yapmayız.
     * shadow property
     * backing field
     * özellikleri kullanıldı.
     * kullanmış olduğumuz orm aracına göre DDD yapımız değişmektedir. NHibernate kullansaydık farklı bir şekilde yapıyı inşa ederdik buradaki.
    */
    public class Order:Entity,IAggregateRoot
    {
        public DateTime CreatedDate { get; private set; }
        public Address Address { get; private set; } //owned entity types konusunu araştır. Address bir owned type olarak inşa edildi.
        public string BuyerId { get; private set; }

        //relations
        private readonly List<OrderItem> _orderItems; //backing-field olur. Eğer ki entity framework core içerisinde okuma ve yazma işlemini property'den ziyade bir field'dan yapıyorsak buna --> backing-field olarak adlandırılır. amacımız encapsulation'ı arttırmak. Çünkü Order üzerinden kimse OrderItem'a data eklemesin, benim metodum üzerinden eklesin.
        public IReadOnlyCollection<OrderItem> OrderItems => _orderItems; //direkt olarak kapsülleme işlemi yaptık.
        public Order()
        {
            //alt satırda custom ctor yaptığımız için bu default olarak gelmez, bunu kendimiz elle yazmak zorundayız.
        }
        public Order(string buyerId, Address address)
        {
            _orderItems = new List<OrderItem>();
            CreatedDate= DateTime.Now;
            BuyerId= buyerId;
            Address= address;
        }

        public void AddOrderItem(string productId, string productName, decimal price, string pictureUrl)
        {
            var existProduct = _orderItems.Any(x => x.ProductId == productId);
            if (!existProduct)
            {
                var newOrderItem = new OrderItem(productId, productName, pictureUrl, price);

                _orderItems.Add(newOrderItem);
            }
        }
        public decimal GetTotalPrice => _orderItems.Sum(x => x.Price);
    }
}
