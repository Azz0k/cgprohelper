using GateKeeper.Core.Application;
using GateKeeper.Core.Models.ApiModels;
using GateKeeper.Core.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GateKeeper.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class LocalMonitoredEmailsController : ControllerBase
    {
        private LocalMonitoredEmailsApplication app;
        public LocalMonitoredEmailsController(LocalMonitoredEmailsApplication app)
        {
            this.app = app;
        }
        // GET: api/<LocalMonitoredEmailsController>
        [HttpGet]
        public async Task<IEnumerable<LocalMonitoredEmails>> Get()
        {
            return await app.GetAllRecordsAsync<LocalMonitoredEmails>();
        }
        [HttpPost]
        // POST api/<LocalMonitoredEmailsController>
        public async Task<int> Post([FromBody] AddLocalMonitoredEmailsRequest value)
        {
            return await app.AddAsync(value);
        }

        // PUT api/<LocalMonitoredEmailsController>
        [HttpPut]
        public async Task<StatusCodeResult> Put([FromBody] UpdateLocalMonitoredEmailsRequest value)
        {
            int code = await app.UpdateAsync(value);
            return StatusCode(code);
        }

        // DELETE api/<LocalMonitoredEmailsController>/5
        [HttpDelete("{id}")]
        public async Task<StatusCodeResult> Delete(int id)
        {
            int code = await app.DeleteAsync<LocalMonitoredEmails>(id);
            return StatusCode(code);
        }
    }
}
