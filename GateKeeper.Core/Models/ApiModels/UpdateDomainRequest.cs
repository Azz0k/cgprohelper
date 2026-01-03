using System;
using System.Collections.Generic;
using System.Text;

namespace GateKeeper.Core.Models.ApiModels
{
    public class UpdateDomainRequest
    {
        public int Id { get; set; }
        public string Domain { get; set; }
    }
}
