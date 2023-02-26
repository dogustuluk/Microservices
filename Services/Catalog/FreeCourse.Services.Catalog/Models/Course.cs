using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace FreeCourse.Services.Catalog.Models
{
    public class Course
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        
        public string Name { get; set; }
        public string Description { get; set; }

        [BsonRepresentation(BsonType.Decimal128)]
        public decimal Price { get; set; }

        public string UserId { get; set; } //string tutuyoruz çünkü identity tarafında kullanıcı id'sini string ve random bir değerle(guid) olarak tutucam.
                                           //UserId'yi string veya random bir değerle tutmak güvenlik açısından daha doğru integer değere göre.

        public string Picture { get; set; }

        [BsonRepresentation(BsonType.DateTime)]
        public DateTime CreatedTime { get; set; }

        [BsonRepresentation(BsonType.ObjectId)] //category sınıfının id'si objectId olarak geçtiğinden burada mutlaka vermemiz gerekmektedir.
        public string CategoryId { get; set; }

        //one to one ilişki
        public Feature Feature { get; set; }

        [BsonIgnore] //burayı kodlama esnasında kullanıyor olucaz. Yani product'ları dönerken Category'leri de dönmek istiyorum. Yalnız bunun mongodb tarafında herhangi bir karşılığı olmasının istemiyorum. Dolayısıyla bsonignore ile gözardı etmesini sağlıyoruz.
        public Category Category { get; set; }
    }
}
