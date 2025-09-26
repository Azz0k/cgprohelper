using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGProToCCAddressHelper.Models
{
    public class AppSettings
    {
        public AppConnectionSettings ConnectionSettings { get; init; }
        public string baseDir { get; init; } //The directory where CGPro places files
        public string currentDir { get; init; } //The directory for your own needs 
        public string[] allowedDomains { get; init; }
        public string emailsLocalFullFileName { get; init; }
        public int updateIntervalInSeconds { get; init; }
    }

    public class AppConnectionSettings
    {
        public string host { get; init; }
        public string login { get; init; }

        public string password { get; init; }
        public string emailsFullFileName { get; init; }
    }
}
