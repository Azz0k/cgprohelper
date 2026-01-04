using System;
using System.Collections.Generic;
using System.Text;

namespace GateKeeper.Core.Models.ApiModels
{
    public class AddAllowedDomainsRequest
    {
        public List<string>? Domain { get; set; }
    }
}
