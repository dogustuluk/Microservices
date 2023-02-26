using System;
using System.Collections.Generic;
using System.Linq;

namespace FreeCourse.Web.Models.Baskets
{
    public class BasketViewModel
    {
        public BasketViewModel()
        {
            _basketItems = new List<BasketItemViewModel>();
        }
        public string UserId { get; set; }
        public string DiscountCode { get; set; }
        public int? DiscountRate { get; set; }
        private List<BasketItemViewModel> _basketItems;

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
            get => _basketItems.Sum(x => x.GetCurrentPrice);
        }
        //indirim olup olmadığının kontrolü
        public bool HasDiscount
        {
            get => !string.IsNullOrEmpty(DiscountCode) && DiscountRate.HasValue;
        }
        public void CancelDiscount()
        {
            DiscountCode = null;
            DiscountRate = null;
        }
        public void ApplyDiscount(string code, int rate)
        {
            DiscountCode = code;
            DiscountRate = rate;
        }
    }
}
