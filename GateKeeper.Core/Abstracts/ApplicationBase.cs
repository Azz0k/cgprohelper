using GateKeeper.Core.Interfaces;
using GateKeeper.Core.Models.ApiModels;
using GateKeeper.Core.Services;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace GateKeeper.Core.Abstracts
{
    public abstract class ApplicationBase: IApplication
    {
        private IDataBaseService dbservice;
        public ApplicationBase(DatabaseService dbservice)
        {
            this.dbservice = dbservice;
        }


        public virtual async Task<int> DeleteAsync<T>(int id) where T : class
        {
            return await dbservice.DeleteAsync<T>(id)?204:404;
        }

        public virtual async Task<List<T>> GetAllRecordsAsync<T>() where T : class
        {
            return await dbservice.ReadAllAsync<T>();

        }
    }
}
