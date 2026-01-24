using GateKeeper.Core.Application;
using GateKeeper.Core.Models.ApiModels;
using GateKeeper.Core.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;


namespace GateKeeper.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private UserApplication app;
        public UsersController(UserApplication application) 
        {
            app = application;
        }
        // GET: api/Users
        [HttpGet]
        public async Task<IEnumerable<UserDTO>> Get()
        {
            List<User> users = await app.GetAllRecordsAsync<User>();
            return users.Select(x => new UserDTO {UserName = x.UserName, FullName = x.FullName, Enabled = x.Enabled, Id= x.Id, IsAdmin = x.IsAdmin });
        }
        // POST api/Users
        [HttpPost]
        public async Task<int> Post([FromBody] AddUserRequest request)
        {
            return await app.AddAsync(request);
        }
        // PUT api/Users
        [HttpPut]
        public async Task<StatusCodeResult> Put([FromBody] UpdateUserRequest request)
        {
            int code = await app.UpdateAsync(request);  
            return StatusCode(code);
        }
        // DELETE api/Users/<id>
        [HttpDelete("{id}")]
        public async Task<StatusCodeResult> Delete(int id)
        {
            int code = await app.DeleteAsync<User>(id);
            return StatusCode(code);
        }
    }
}
