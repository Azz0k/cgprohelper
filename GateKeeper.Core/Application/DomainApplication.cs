using GateKeeper.Core.Abstracts;
using GateKeeper.Core.Interfaces;
using GateKeeper.Core.Models;
using GateKeeper.Core.Models.ApiModels;
using GateKeeper.Core.Models.Entities;
using GateKeeper.Core.Services;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using static GateKeeper.Core.Utils.Utils;

namespace GateKeeper.Core.Application
{
    public class DomainApplication: ApplicationBase
    {
        private IDataBaseService dbservice;
        public DomainApplication(DatabaseService dbservice):base(dbservice)
        { 
            this.dbservice = dbservice;
        }
        public async Task<Dictionary<int,HashSet<string>>> AddAsync(AddDomainRequest domains)
        {
            HashSet<string> okAdded = [];
            HashSet<string> error = [];
            Dictionary<int, HashSet<string>> result = new();
            result[201] = okAdded;
            result[400] = error;
            if (domains?.Domain == null) return result;
            foreach (var domain in domains.Domain)
            {
                if (isDomainPatternValid(domain) && !okAdded.Contains(domain))
                {
                    bool isEntityExists = await dbservice.ExistsAsync<AllowedDomains>(d => d.Domain == domain);
                    if (!isEntityExists)
                    {
                        await dbservice.CreateAsync(new AllowedDomains(domain));
                        okAdded.Add(domain);
                    }
                }
                if (!okAdded.Contains(domain))
                {
                    error.Add(domain); 
                }
                
            }
            return result;
        }

        public async Task<int> UpdateAsync(UpdateDomainRequest request)
        {
            if (!isDomainPatternValid(request.Domain)) return 400;
            return await dbservice.UpdateAsync<AllowedDomains>(request.Id, (AllowedDomains domain) => domain.Domain = request.Domain)?200:404;
        }
    }
}
