using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace FreeCourse.Shared.Dtos
{
    /*desc
     * generic yapıyı herhangi bir where ifadesi ile kısıtlamıyoruz. Buraya bir int değer de gelebilir, bir class ya da struct yapı da gönderilebilir.
     */
    public class Response<T>
    {
        public T Data { get;  set; }

        /*jsonignore
         * kendi içerimizde kullanmak istiyoruz. response'ın dönüş tipini belirlerken buradan faydalanıyoruz ama bu property'nin response'ın içerisinde olmasına gerek yok. bundan dolayı jsonignore ile işaretleme yapıyoruz.
         */
        [JsonIgnore]
        public int StatusCode { get;  set; }

        [JsonIgnore]
        public bool IsSuccessfull { get;  set; }

        public List<string> Errors { get; set; }

        /*response dto nesneleri üretmek için static methodlar yazıyoruz.
         * static factory metotlar sınıfın içerisinde tanımlarsak direkt olarak nesne oluşturmaya yardımcı olurlar. diğer türlü yaparsak interface'lerini de oluşturmamız gerekir.
         * isim olarak static factory method olarak geçer.
         */
        public static Response<T> Success(T data, int statusCode)
        {
            return new Response<T> { Data = data, StatusCode = statusCode, IsSuccessfull = true };
        }

        public static Response<T> Success(int statusCode)
        {
            return new Response<T> { Data = default(T), StatusCode = statusCode, IsSuccessfull = true };
        }

        public static Response<T> Fail(List<string> errors, int statusCode)
        {
            return new Response<T>
            {
                Errors = errors,
                StatusCode = statusCode,
                IsSuccessfull = false
            };
        }

        public static Response<T> Fail(string error, int statusCode)
        {
            return new Response<T> { Errors = new List<string>() { error }, StatusCode = statusCode, IsSuccessfull = false };

        }
    }
}