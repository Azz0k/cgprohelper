using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGProToCCAddressHelper.Models
{
    public class AppSettings
    {
        public required AppConnectionSettings ConnectionSettings { get; init; }
        public required string baseDir { get; init; } //The directory where CGPro places files
        public required string currentDir { get; init; } //The directory for your own needs 
        public required string[] allowedDomains { get; init; }
        public required string emailsLocalFullFileName { get; init; }
        public required int updateIntervalInSeconds { get; init; }
    }

    public class AppConnectionSettings
    {
        public required string host { get; init; }
        public required string login { get; init; }

        public required string password { get; init; }
        public required string emailsFullFileName { get; init; }
    }
}
