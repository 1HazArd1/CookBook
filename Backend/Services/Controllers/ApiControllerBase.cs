using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace iMocha.Talent.Analytics.Services.Controllers
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
