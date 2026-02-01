using GateKeeper.Core.Models.ApiModels;
using GateKeeper.Core.Models.Entities;
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
        public static bool isAddUserRequestValid(AddUserRequest request)
        {
            return true;
        }
        public static bool isUpdateUserRequestValid(UpdateUserRequest request)
        {
            if (request.Id<=0) return false;
            return true;
        }
        public static string GenerateReceivedDate()
        {
            return DateTime.Now.ToString("yyyy-MM-dd");
        }
        public static string GenerateReceivedTime()
        {
            return DateTime.Now.ToString("HH:mm");
        }
        public static string GenerateDeprecatedDate()
        {
            return DateTime.Now.AddDays(-8).ToString("yyyy-MM-dd");
        }

    }
}
