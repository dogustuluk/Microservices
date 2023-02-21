using System;

namespace FreeCourse.Web.Models.Discounts
{
    public class DiscountViewModel
    {
        public string UserId { get; set; } //aslında buna da gerek yok.
        public int Rate { get; set; }
        public string Code { get; set; }
    }
}
