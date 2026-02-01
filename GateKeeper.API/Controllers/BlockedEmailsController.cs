using GateKeeper.Core.Application;
using GateKeeper.Core.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GateKeeper.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BlockedEmailsController : ControllerBase
    {
        private BlockedEmailsApplication app;
        public BlockedEmailsController(BlockedEmailsApplication app)
        {
            this.app = app;
        }

        // GET: api/<BlockedEmailsController>
        [HttpGet]
        public async Task<IEnumerable<BlockedEmails>> Get()
        {
            return await app.GetAllRecordsAsync<BlockedEmails>();
        }

    }
}
