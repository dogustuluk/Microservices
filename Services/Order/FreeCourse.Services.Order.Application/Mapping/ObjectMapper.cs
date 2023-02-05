using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeCourse.Services.Order.Application.Mapping
{
    public static class ObjectMapper
    {
        /*new Lazy<IMapper>(()=>{})
         * bu function bir delegedir.
         * öyle bir metodu işaret ediyor ki IMapper hiçbir şey almıyor ve geriye de IMapper dönecektir.
         */
        private static readonly Lazy<IMapper> lazy = new Lazy<IMapper>(() =>
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CustomMapping>();
            });
            return config.CreateMapper();
        });

        public static IMapper Mapper => lazy.Value; //-> Mapper çağırılana kadar yukarıdaki kodlar hiç yüklenmeyecek. 
    }
}
