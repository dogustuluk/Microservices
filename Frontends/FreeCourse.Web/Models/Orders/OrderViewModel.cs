using System.Collections.Generic;
using System;

namespace FreeCourse.Web.Models.Orders
{
    public class OrderViewModel
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }

       // public Address Address { get; set; } //Ödeme geçmişinde adres alanına ihtiyaç olmadığından dolayı almadık. OrderDto'dan adres geliyor.
        public string BuyerId { get; set; }

        public List<OrderItemViewModel> OrderItems { get; set; }
    }
}
