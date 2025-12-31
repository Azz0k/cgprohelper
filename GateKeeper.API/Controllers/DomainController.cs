using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage;
using GateKeeper.Core.Services;
using GateKeeper.Core.Interfaces;
using System.Threading.Tasks;
using GateKeeper.Core.Models.Entities;
using GateKeeper.Core.Models.ApiModels;
using GateKeeper.Core.Application;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GateKeeper.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DomainController : ControllerBase
    {
        private DomainApplication app { get; set; }
        public DomainController(DomainApplication app)
        {
            this.app = app;
        }
        // GET: api/<APIController>
        [HttpGet]
        public async Task<IEnumerable<AllowedDomains>> Get()
        {
            return await app.GetAllRecordsAsync<AllowedDomains>();
        }

      

        // POST api/<APIController>
        [HttpPost]
        public async Task Post([FromBody] AddDomainRequest value)
        {
            await app.AddAsync<AllowedDomains>(value);
        }

        // GET api/<APIController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }
        // DELETE api/<APIController>/5
        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            await app.DeleteAsync<AllowedDomains>(id);
        }
    }
}
