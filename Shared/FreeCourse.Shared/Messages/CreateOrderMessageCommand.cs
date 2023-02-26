using System;
using System.Collections.Generic;
using System.Text;

namespace FreeCourse.Shared.Messages
{
    /*desc
     *FakePayment mikroservisi bu mesajı alacak. Mesajı işleyecek olan ise Order mikroservisi.
     *Mesajı işleyecek olan Order mikroservisi içerisinden OrderController'a geldiğimizde, SaveOrder metodu CreateOrderCommand parametresini almaktadır. Bu parametrenin implementasyonuna gittiğimizde ise bizden; BuyerId, OrderItems(bunun da implementasyonuna gitmemiz lazım) ve Address((bunun da implementasyonuna gitmemiz lazım)) propertylerini istemektedir. 
     */
    public class CreateOrderMessageCommand
    {
        public string BuyerId { get; set; }
        public List<OrderItem> OrderItems { get; set; }
        public Address Address { get; set; }
    }
    public class OrderItem
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string PictureUrl { get; set; }
        public Decimal Price { get; set; }
    }
    public class Address
    {
        public string Province { get; set; }
        public string District { get; set; }
        public string Street { get; set; }
        public string ZipCode { get; set; }
        public string Line { get; set; }
    }
}
