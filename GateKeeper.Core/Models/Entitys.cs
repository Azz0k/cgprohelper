using System.ComponentModel.DataAnnotations;

namespace GateKeeper.Core.Models
{
    public class ForeingAddresses
    {
        public required string Email { get; set; } 
        public required string ReceivedDate { get; set; }
    }
    public class LocalMonitoredAddresses
    {
        public required string Email { get; set; } 
        public bool IsReplyAllowed { get; set; }
    }
    public class AllowedDomains
    {
        public required string Domain { get; set; } 
    
    }
}
