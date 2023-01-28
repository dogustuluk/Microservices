using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreeCourse.Shared.Services
{
    //Kullanıcının id'sini her almak istediğimde mikroservislerin içerisinde metot yazmamak için shared olarak bunu yazarız.
    public class SharedIdentityService : ISharedIdentityService
    {
        //kullanıcının claim'lerine ulaşmak için.
        private IHttpContextAccessor _httpContextAccessor;

        public SharedIdentityService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        //_httpContextAccessor.HttpContext.User.Claims.Where(x => x.Type == "sub").FirstOrDefault().Value; yerine altta where ifadesi olmadan da kullanabiliriz.
        public string GetUserId => _httpContextAccessor.HttpContext.User.FindFirst("sub").Value;

    }
}
