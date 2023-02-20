using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace FreeCourse.Web.Models.Catalog
{
    public class CourseUpdateInput
    {
        public string Id { get; set; }
        [Display(Name = "Kurs Adı")]
        [Required]
        public string Name { get; set; }
        [Display(Name = "Kurs Açıklama")]
        [Required]
        public string Description { get; set; }
        [Display(Name = "Kurs Fiyatı")]
        [Required]
        public decimal Price { get; set; }

        public string UserId { get; set; }
        public string Picture { get; set; }
        [Display(Name = "Kurs Kategorisi")]
        [Required]
        public string CategoryId { get; set; }

        public FeatureViewModel Feature { get; set; }
        [Display(Name = "Kurs Resmi")]
        public IFormFile PhotoFormFile { get; set; }
    }
}
