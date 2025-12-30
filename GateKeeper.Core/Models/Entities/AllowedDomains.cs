using System;
using System.Collections.Generic;
using System.Text;

namespace GateKeeper.Core.Models.Entities
{
    public class AllowedDomains
    {
        public int Id { get; set; }
        public string Domain { get; set; }
        public AllowedDomains(string domain) 
        { 
            Domain = domain;
        }
    }
}
