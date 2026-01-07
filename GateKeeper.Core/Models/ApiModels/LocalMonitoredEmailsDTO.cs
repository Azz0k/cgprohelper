using System;
using System.Collections.Generic;
using System.Text;

namespace GateKeeper.Core.Models.ApiModels
{
    public class LocalMonitoredEmailsDTO
    {
        public int Id { get; set; }
        public required string Email { get; set; }
        public bool IsReplyAllowed { get; set; }

    }
}
