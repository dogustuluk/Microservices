using FreeCourse.Shared.Dtos;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace FreeCourse.Shared.ControllerBases
{
    /*ControllerBase
     * burada Controller sınıfını miras alamayız çünkü aspnetcore'a ait olduğu için ama bu durumu aşmanın bir yolu vardır. burada projeye sağ tıklayıp "edit project file" dedikten sonra elle framework'ü eklememiz gerekiyor. Yazılacak olan kısım --> <ItemGroup>
		<FrameworkReference Include="Microsoft.AspNetCore.App" />
		</ItemGroup>
     */
    public class CustomBaseController : ControllerBase
    {
        /*CreateActionResultInstance
         * api controller'larında sürekli olarak status code dönmemize gerek kalmadan işlemi daha da sadeleştirebiliriz. Burada response'ın status code'unu alıyoruz bu sayede controller'larda return ok, return bad request, 404 not found gibi dönmemize gerek kalmıyor.
         */
        public IActionResult CreateActionResultInstance<T>(Response<T> response)
        {
            return new ObjectResult(response)
            {
                StatusCode = response.StatusCode
            };
        }
    }
}
