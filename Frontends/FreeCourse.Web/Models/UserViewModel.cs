using System.Collections.Generic;

namespace FreeCourse.Web.Models
{
    public class UserViewModel
    {
        //burada önce IdentityServerAPI'den ne geldiğine bakalım
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string City { get; set; }

        //yukarıdaki property'leri bize tek tek dönecek bir metot yazalım
        public IEnumerable<string> GetUserProps()
        {
            yield return UserName;
            yield return Email;
            yield return City;
        }
    }
}
