using System;
using System.Collections.Generic;
using System.Text;

namespace FreeCourse.Shared.Messages
{
    public class CourseNameAndPriceChangedForBasketEvent
    {
        public string CourseId { get; set; }
        public string UserId { get; set; }
        public string UpdatedBasketCourseName { get; set; }
        public decimal UpdatedCoursePrice { get; set; }
    }
}
