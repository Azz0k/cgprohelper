using System;
using System.Collections.Generic;
using System.Text;

namespace GateKeeper.Core.Models.ApiModels
{
    public class LoginRequest
    {
        public required string Login {  get; set; }
        public required string Password { get; set; }
    }
}
