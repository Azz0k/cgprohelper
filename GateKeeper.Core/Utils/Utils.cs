using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace GateKeeper.Core.Utils
{
    public static class Utils
    {
        public static bool isDomainPatternValid(string? domain) 
        {
            if (domain == null) return false;
            string pattern = @"^(([a-z0-9\-]+)|\*)(\.(([a-z0-9\-]+)|\*))*$";
            var match = Regex.Match(domain, pattern,RegexOptions.IgnoreCase);
            return match.Success;
        }
    }
}
