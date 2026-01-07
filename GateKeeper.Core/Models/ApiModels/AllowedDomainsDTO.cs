using System;
using System.Collections.Generic;
using System.Text;

namespace GateKeeper.Core.Models.ApiModels
{
    public class AllowedDomainsDTO
    {
        public int Id { get; set; }
        public required string Domain { get; set; }
    }
}
