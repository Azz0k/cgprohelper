using System;
using System.Collections.Generic;
using System.Text;

namespace GateKeeper.Core.Models.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Hash { get; set; }
        public string FullName { get; set; }
        public bool Enabled { get; set; }


    }
}
