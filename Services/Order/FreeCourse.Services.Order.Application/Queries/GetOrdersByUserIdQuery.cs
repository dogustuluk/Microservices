using FreeCourse.Services.Order.Application.Dtos;
using FreeCourse.Shared.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeCourse.Services.Order.Application.Queries
{
    class GetOrdersByUserIdQuery : IRequest<Response<List<OrderDto>>>
    {
        public string UserId { get; set; } //controller içerisinde göndericez daha sonra mediatR'da böyle bir sınıfla karşılaştığında(GetOrdersByUserIdQuery) bunu handle edecek olan sınıfı otomatik olarak bulmuş olacak. Yani ben controller tarafında bu sınıftan bir nesne örneği aldığımda ve UserId'sini doldurduğumda, mediatR'a gönderdiğimde burada bu sınıfı handle edecek olan sınıfı kendisi otomatik bir şekilde mediator design pattern ile beraber bulmuş olacak.
         
    }
}
