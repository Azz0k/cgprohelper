using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGProToCCAddressHelper
{
    public class AppSettings
    {
        public AppConnectionSettings ConnectionSettings { get; init; }
        public string baseDir { get; init; }
        public string[] allowedDomains { get; init; }
    }

    public class AppConnectionSettings
    {
        public string host { get; init; }
        public string login { get; init; }

        public string password { get; init; }
        public string emailsFullFileName { get; init; }
    }
}
