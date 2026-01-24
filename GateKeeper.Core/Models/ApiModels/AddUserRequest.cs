using System;
using System.Collections.Generic;
using System.Text;

namespace GateKeeper.Core.Models.ApiModels
{
    public class AddUserRequest
    {
        public required string UserName { get; set; }
        public required string Password { get; set; }
        public required string FullName { get; set; }
        public bool Enabled { get; set; } = true;
        public bool IsAdmin { get; set; } = false;
    }
}
