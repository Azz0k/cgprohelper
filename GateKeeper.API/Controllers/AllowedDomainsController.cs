using GateKeeper.Core.Application;
using GateKeeper.Core.Interfaces;
using GateKeeper.Core.Models.ApiModels;
using GateKeeper.Core.Models.Entities;
using GateKeeper.Core.Services;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage;
using System.Threading.Tasks;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GateKeeper.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AllowedDomainsController : ControllerBase
    {
        private AllowedDomainsApplication app { get; set; }
        public AllowedDomainsController(AllowedDomainsApplication app)
        {
            this.app = app;
        }
        // GET: api/<APIController>
        [HttpGet]
        public async Task<IEnumerable<AllowedDomainsDTO>> Get()
        {
            return await app.GetAllRecordsAsync();
        }
        // PUT api/<APIController>
        [HttpPut]
        public async Task<StatusCodeResult> Put([FromBody] UpdateDomainRequest value)
        {
            int code = await app.UpdateAsync(value);
            return StatusCode(code);
        }


        // POST api/<APIController>
        [HttpPost]
        public async Task<Dictionary<int, HashSet<AllowedDomainsDTO>>> Post([FromBody] AddAllowedDomainsRequest value)
        {
            return await app.AddAsync(value);
        }

        // DELETE api/<APIController>/5
        [HttpDelete("{id}")]
        public async Task<StatusCodeResult> Delete(int id)
        {
            int code = await app.DeleteAsync<AllowedDomains>(id);
            return StatusCode(code);
        }
    }
}
