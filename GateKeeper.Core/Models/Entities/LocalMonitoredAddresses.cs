using System;
using System.Collections.Generic;
using System.Text;

namespace GateKeeper.Core.Models.Entities
{
    public class LocalMonitoredAddresses
    {
        public int Id { get; set; } 
        public required string Email { get; set; }
        public bool IsReplyAllowed { get; set; }
    }
}
