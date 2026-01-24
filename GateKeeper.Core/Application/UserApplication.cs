using GateKeeper.Core.Abstracts;
using GateKeeper.Core.Context;
using GateKeeper.Core.Interfaces;
using GateKeeper.Core.Models.ApiModels;
using GateKeeper.Core.Services;
using GateKeeper.Core.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using static GateKeeper.Core.Utils.Utils;

namespace GateKeeper.Core.Application
{
    public class UserApplication: ApplicationBase
    {
        private IDataBaseService dbservice;
        public UserApplication(DatabaseService dbservice):base(dbservice) 
        {
            this.dbservice = dbservice;
        }
        public async Task<int> AddAsync(AddUserRequest request)
        {
            if (!isAddUserRequestValid(request))
            {
                return -1;
            }
            User? existingUser = await dbservice.FindAsync<User>(user => user.UserName == request.UserName);
            if (existingUser != null)
            {
                return existingUser.Id;
            }
            string hash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            User newUser = new() { UserName = request.UserName, Hash = hash, FullName = request.FullName, Enabled = request.Enabled, IsAdmin = request.IsAdmin};
            newUser = await dbservice.CreateAsync(newUser);
            return newUser.Id;
        }
        public async Task<int> UpdateAsync(UpdateUserRequest request)
        {
            if (!isUpdateUserRequestValid(request)) return 400;
            Action<User> updateDelegate = delegate (User u)
            {
                u.UserName = request.UserName;
                u.FullName = request.FullName;
                u.Enabled = request.Enabled;
                u.IsAdmin = request.IsAdmin;
                if (request.Password != null)
                {
                    string hash = BCrypt.Net.BCrypt.HashPassword(request.Password);
                    u.Hash = hash;
                }
            };
            bool result = false;
            try
            {
                result = await dbservice.UpdateAsync(request.Id, updateDelegate);
            }
            catch
            {
                return 400;
            }
            return result ? 200 : 404;
        }
    }

}
