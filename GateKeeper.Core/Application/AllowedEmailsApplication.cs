using GateKeeper.Core.Abstracts;
using GateKeeper.Core.Interfaces;
using GateKeeper.Core.Models.Entities;
using GateKeeper.Core.Services;
using GateKeeper.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GateKeeper.Core.Application
{
    public class AllowedEmailsApplication : ApplicationBase
    {
        private IDataBaseService dbservice;
        public AllowedEmailsApplication(DatabaseService dbservice) : base(dbservice)
        {
            this.dbservice = dbservice;
        }
    
        public async Task<bool> IsEmailExists(string email)
        {
            return await dbservice.FindAsync<AllowedEmails>(e => e.Email == email)==null?false:true;
        }
        public async Task SyncTable(HashSet<string> ftpEmails)
        {
            List<AllowedEmails> dbAddressesList = await dbservice.ReadAllAsync<AllowedEmails>();
            HashSet<string> dbAddresses = dbAddressesList.Select(x => x.Email).ToHashSet(StringComparer.OrdinalIgnoreCase);
            var toRemove = dbAddresses.Except(ftpEmails);
            foreach (var email in toRemove)
            {
                await dbservice.DeleteAsync<AllowedEmails>(dbAddressesList.Find(x=>x.Email == email).Id);
            }
            var toAdd = ftpEmails.Except(dbAddresses);
            await dbservice.BulkInsertAsync<AllowedEmails>(toAdd.Select(x=>new AllowedEmails { Email=x}));
        }
    }
}
