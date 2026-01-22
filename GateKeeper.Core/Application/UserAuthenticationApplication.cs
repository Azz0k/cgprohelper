using GateKeeper.Core.Context;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace GateKeeper.Core.Application
{
    public class UserAuthenticationApplication
    {
        private AddressesDbContext dbContext;
        public UserAuthenticationApplication(AddressesDbContext dbContext) 
        { 
            this.dbContext = dbContext;
        }
    }
}
