using GateKeeper.Core.Application;
using GateKeeper.Core.Models.ApiModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GateKeeper.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private UserAuthenticationApplication application;
        public AuthenticationController(UserAuthenticationApplication application)
        {
            this.application = application;
        }
        [HttpPost]
        public async Task<IResult> Post([FromBody] LoginRequest request)
        {
            string? res = await application.Authenticate(request);
            if (res == null)
            {
                return Results.Unauthorized();
            }
            return Results.Ok(res);
        }

    }
}
