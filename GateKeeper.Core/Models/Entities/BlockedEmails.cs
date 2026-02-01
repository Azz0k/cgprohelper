using System;
using System.Collections.Generic;
using System.Text;

namespace GateKeeper.Core.Models.Entities
{
    public class BlockedEmails
    {
        public int Id { get; set; }
        public required string SenderEmail { get; set; }
        public required string RecipientEmail { get; set; }
        public required string Date { get; set; }
        public required string Time { get; set; }
    }
}
