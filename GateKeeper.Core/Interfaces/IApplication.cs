using GateKeeper.Core.Models.ApiModels;
using GateKeeper.Core.Models.Entities;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace GateKeeper.Core.Interfaces
{
    public interface IApplication
    {
        Task<int> DeleteAsync<T>(int id) where T : class;
        Task<List<T>> GetAllRecordsAsync<T>() where T: class;
    }
}
