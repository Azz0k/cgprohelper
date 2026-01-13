using GateKeeper.Core.Abstracts;
using GateKeeper.Core.Interfaces;
using GateKeeper.Core.Models;
using GateKeeper.Core.Models.ApiModels;
using GateKeeper.Core.Models.Entities;
using GateKeeper.Core.Services;
using GateKeeper.Core.Utils;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using static GateKeeper.Core.Utils.Utils;

namespace GateKeeper.Core.Application
{
    public class AllowedDomainsApplication: ApplicationBase
    {
        private IDataBaseService dbservice;
        public AllowedDomainsApplication(DatabaseService dbservice):base(dbservice)
        { 
            this.dbservice = dbservice;
        }
        public async Task<List<AllowedDomainsDTO>> GetAllRecordsAsync()
        {
            var res = await dbservice.QueryAsync<AllowedDomains>((AllowedDomains d) => d.isManuallyAdded == true);
            return  res.Select(d=> new AllowedDomainsDTO() { Id = d.Id, Domain = d.Domain }).ToList();
        }
        public async Task<Dictionary<int,HashSet<AllowedDomainsDTO>>> AddAsync(AddAllowedDomainsRequest domains)
        {
            HashSet<AllowedDomainsDTO> okAdded = new(new DomainDTOComparer());
            HashSet<AllowedDomainsDTO> error = new(new DomainDTOComparer());
            Dictionary<int, HashSet<AllowedDomainsDTO>> result = new();
            result[201] = okAdded;
            result[400] = error;
            if (domains?.Domain == null) return result;
            foreach (string dom in domains.Domain)
            {
                string domain = dom.Trim();
                AllowedDomainsDTO dTO = new() { Domain = domain };
                if (isDomainPatternValid(domain) && !okAdded.Contains(dTO))
                {
                    bool isEntityExists = await dbservice.FindAsync<AllowedDomains>(d => d.Domain == domain) == null ? false:true;
                    if (!isEntityExists)
                    {
                        var createdEntity = await dbservice.CreateAsync(new AllowedDomains(domain, true));
                        dTO.Id = createdEntity.Id;
                        okAdded.Add(dTO);
                    }
                }
                if (!okAdded.Contains(dTO))
                {
                    error.Add(dTO); 
                }
                
            }
            return result;
        }
        public async Task<int> UpdateAsync(UpdateDomainRequest request)
        {
            if (!isDomainPatternValid(request.Domain)) return 400;
            if (request.Id < 0) return 400;
            bool res = false;
            try
            {
                res = await dbservice.UpdateAsync<AllowedDomains>(request.Id, (AllowedDomains domain) => domain.Domain = request.Domain.Trim());
            }
            catch  {
                return 400;
            }
            return res ? 200 : 404;
        }
        public async Task AddAsync(string domain)
        {
            bool isEntityExists = await dbservice.FindAsync<AllowedDomains>(d => d.Domain == domain) == null ? false : true;
            if (!isEntityExists)
            {
                var createdEntity = await dbservice.CreateAsync(new AllowedDomains(domain, false));
            }
        }
        public async Task SyncTable(List<string> ftpDomains)
        {
            var dbAddresses = await dbservice.ReadAllAsync<AllowedDomains>();
            var toRemove = dbAddresses.Where(e => !e.isManuallyAdded && !ftpDomains.Contains(e.Domain)).ToList();
            foreach (var domain in toRemove)
            {
                await dbservice.DeleteAsync<AllowedDomains>(domain.Id);
            }
            var dbDomains = dbAddresses.Select(e => e.Domain).ToHashSet();
            var toAdd = ftpDomains.Where(e => !dbDomains.Contains(e));
            foreach (var item in toAdd)
            {
                await dbservice.CreateAsync<AllowedDomains>(new AllowedDomains(item, false));
            }
        }
    }
}
