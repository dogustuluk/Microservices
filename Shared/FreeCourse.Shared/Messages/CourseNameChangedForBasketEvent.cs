using System;
using System.Collections.Generic;
using System.Text;

namespace FreeCourse.Shared.Messages
{
    public class CourseNameChangedForBasketEvent
    {
        public string CourseId { get; set; }
        public string UserId { get; set; }
        public string UpdatedBasketCourseName { get; set; }
    }
}
