using GateKeeper.Core.Abstracts;
using GateKeeper.Core.Interfaces;
using GateKeeper.Core.Models.Entities;
using GateKeeper.Core.Services;
using System;
using System.Collections.Generic;
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
        public async Task SyncTable(List<string> ftpEmails)
        {
            var dbAddresses = await dbservice.ReadAllAsync<AllowedEmails>();
            var toRemove = dbAddresses.Where(e=>!ftpEmails.Contains(e.Email)).ToList();
            foreach (var email in toRemove)
            {
                await dbservice.DeleteAsync<AllowedEmails>(email.Id);
            }
            var dbEmails = dbAddresses.Select(e => e.Email).ToHashSet();
            var toAdd = ftpEmails.Where(e => !dbEmails.Contains(e));
            foreach (var item in toAdd) 
            {
                await dbservice.CreateAsync<AllowedEmails>(new AllowedEmails() { Email = item });
            }
        }
        public async Task AddAsync(List<string> ftpEmails)
        {
           foreach (var email in ftpEmails)
           {
               bool isEntityExists = await dbservice.FindAsync<AllowedEmails>(d => d.Email == email) == null ? false : true;
               if (!isEntityExists)
               {
                   var createdEntity = await dbservice.CreateAsync(new AllowedEmails() { Email = email});
               }
           }
        }
    }
}
