using GateKeeper.Core.Models.ApiModels;
using GateKeeper.Core.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GateKeeper.Core.Interfaces
{
    public interface IApplication
    {
        Task AddAsync<T>(AddDomainRequest domains) where T : AllowedDomains;
        Task DeleteAsync<T>(int id) where T : class;
        Task<List<T>> GetAllRecordsAsync<T>() where T: class;
    }
}
