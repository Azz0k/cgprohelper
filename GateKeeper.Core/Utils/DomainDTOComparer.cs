using GateKeeper.Core.Models.ApiModels;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace GateKeeper.Core.Utils
{
    public class DomainDTOComparer : IEqualityComparer<AllowedDomainsDTO>
    {
        public bool Equals(AllowedDomainsDTO? x, AllowedDomainsDTO? y)
        {
            if (x == null || y == null) return false;
            return string.Equals(x.Domain, y.Domain, StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode([DisallowNull] AllowedDomainsDTO obj)
        {
            return obj.Domain.GetHashCode();
        }
    }
}
