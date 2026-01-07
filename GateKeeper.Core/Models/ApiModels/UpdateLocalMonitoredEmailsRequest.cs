using System;
using System.Collections.Generic;
using System.Text;

namespace GateKeeper.Core.Models.ApiModels
{
    public class UpdateLocalMonitoredEmailsRequest
    {
        public int Id { get; set; }
        public string? Email { get; set; }
        public bool? IsReplyAllowed { get; set; }
    }
}
