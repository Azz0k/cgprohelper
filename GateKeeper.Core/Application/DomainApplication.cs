using GateKeeper.Core.Interfaces;
using GateKeeper.Core.Models;
using GateKeeper.Core.Models.ApiModels;
using GateKeeper.Core.Models.Entities;
using GateKeeper.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace GateKeeper.Core.Application
{
    public class DomainApplication: IApplication
    {
        private IDataBaseService dbservice;
        public DomainApplication(DatabaseService dbservice) 
        { 
            this.dbservice = dbservice;
        }
        public async Task AddAsync<T>(AddDomainRequest domains) where T : AllowedDomains
        {
            //TODO Validation
            if (domains?.Domain == null) return;
            foreach (var domain in domains.Domain)
            {
                bool isEntiryExist = await dbservice.ExistsAsync<AllowedDomains>(d => d.Domain == domain);
                if (!isEntiryExist)
                {
                    await dbservice.CreateAsync(new AllowedDomains(domain));
                }
            }
        }

        public async Task DeleteAsync<T>(int id) where T : class
        {
            await dbservice.DeleteAsync<T>(id);
        }

        public async Task<List<T>> GetAllRecordsAsync<T>() where T : class
        {
            return await dbservice.ReadAllAsync<T>();

        }
    }
}
