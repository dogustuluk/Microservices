using FreeCourse.Services.Order.Domain.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeCourse.Services.Order.Domain.OrderAggregate
{
    public class Address:ValueObject
    {
        //value object'in durumunu korumak için private set yapıyoruz.
        //state'ini değiştiremezsek yeni nesne oluşturma işlemini ise kendimiz constructor oluşturarak gerçekleştirmiş oluruz.
        //bussiness kodu burada olacaktır.
        public string Province { get; private set; }
        public string District { get; private set; }
        public string Street { get; private set; }
        public string ZipCode { get; private set; }
        public string Line { get; private set; }

        public Address(string province, string district, string street, string zipCode, string line)
        {
            Province = province;
            District = district;
            Street = street;
            ZipCode = zipCode;
            Line = line;
        }

        //Eğer ZipCode ile ilgili bir bussiness kuralımız varsa
        //public void SetZipCode(string zipCode)
        //{
        //    //business kodları//

        //    ZipCode = zipCode;
        //}

        protected override IEnumerable<object> GetEqualityComponents()
        {
            //equals ile beraber bu metodu kullanırsak null olup olmamasına, tipine ve içerisindeki değerlerin eşit olup olmadığına baksın. eğer içerisindeki tüm property'ler birbirine eşitse Equals bana true dönsün.
            yield return Province;
            yield return District;
            yield return Street;
            yield return ZipCode;
            yield return Line;
        }
    }
}
