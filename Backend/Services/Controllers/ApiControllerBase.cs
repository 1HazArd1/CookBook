using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CookBook.Services.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ApiControllerBase : ControllerBase
    {
    }

    [Authorize]
    public class ApiAuthorizeControllerBase : ApiControllerBase
    {
    }
}
