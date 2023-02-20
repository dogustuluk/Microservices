using System;
using System.Collections.Generic;
using System.Linq;

namespace FreeCourse.Web.Models.Baskets
{
    public class BasketViewModel
    {
        public string UserId { get; set; }
        public string DiscountCode { get; set; }
        public int? DiscountRate { get; set; }
        private List<BasketItemViewModel> _basketItems { get; set; }

        public List<BasketItemViewModel> BasketItems
        {
            get
            {
                if (HasDiscount)
                {
                    //örn kurs 100tl, indirim oranı %10 ise
                    //indirimli fiyatı al
                    _basketItems.ForEach(x =>
                    {
                        var discountPrice = x.Price * ((decimal)DiscountRate.Value / 100); //10 gelir
                        x.AppliedDiscount(Math.Round(x.Price - discountPrice, 2)); //90,00 yazacak.
                    });
                }
                return _basketItems;
            }
            set
            {
                _basketItems = value;
            }
        }
        public decimal TotalPrice
        {
            get => _basketItems.Sum(x => x.GetCurrentPrice * x.Quantity);
        }
        //indirim olup olmadığının kontrolü
        public bool HasDiscount
        {
            get => !string.IsNullOrEmpty(DiscountCode);
        }
    }
}
