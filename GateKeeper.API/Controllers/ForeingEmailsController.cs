using GateKeeper.Core.Application;
using GateKeeper.Core.Models.ApiModels;
using GateKeeper.Core.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GateKeeper.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ForeingEmailsController : ControllerBase
    {
        private ForeingEmailsApplication app;
        public ForeingEmailsController(ForeingEmailsApplication app)
        {
            this.app = app;
        }

        // GET: api/<ForeingEmailsController>
        [HttpGet]
        public async Task<IEnumerable<ForeingEmails>> Get()
        {
            return await app.GetAllRecordsAsync<ForeingEmails>();
        }


        // POST api/<ForeingEmailsController>
        [HttpPost]
        public async Task<int> Post([FromBody] AddForeingEmailRequest value)
        {
            return await app.AddAsync(value);
        }

 
        // DELETE api/<ForeingEmailsController>/5
        [HttpDelete("{id}")]
        public async Task<StatusCodeResult> Delete(int id)
        {
            int code = await app.DeleteAsync<ForeingEmails>(id);
            return StatusCode(code);
        }
    }
}
