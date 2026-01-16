using GateKeeper.Core.Models.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace GateKeeper.Core.Utils
{
    public class AllowedEmailsComparer: IEqualityComparer<AllowedEmails>
    {
        public bool Equals(AllowedEmails? x, AllowedEmails? y)
        {
            if (x == null || y == null) return false;
            return string.Equals(x.Email, y.Email, StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode([DisallowNull] AllowedEmails obj)
        {
            return obj.Email.GetHashCode();
        }
    }
 
}
