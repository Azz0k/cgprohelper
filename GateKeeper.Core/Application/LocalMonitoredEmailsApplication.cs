using GateKeeper.Core.Abstracts;
using GateKeeper.Core.Interfaces;
using GateKeeper.Core.Models.ApiModels;
using GateKeeper.Core.Models.Entities;
using GateKeeper.Core.Services;
using GateKeeper.Core.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace GateKeeper.Core.Application
{
    public class LocalMonitoredEmailsApplication: ApplicationBase
    {
        private IDataBaseService dbservice;
        public LocalMonitoredEmailsApplication(DatabaseService dbservice) : base(dbservice)
        {
            this.dbservice = dbservice;
        }
        public async Task<int> AddAsync(AddLocalMonitoredEmailsRequest request)
        {
            if (request.Email == null) return -1;
            string email = request.Email.Trim();
            LocalMonitoredEmails? emailInBase = await dbservice.FindAsync<LocalMonitoredEmails>(e  => e.Email == email);
            if (emailInBase == null)
            {
                LocalMonitoredEmails newEmail = new() { Email = email , IsReplyAllowed = request.IsReplyAllowed??false};
                newEmail = await dbservice.CreateAsync(newEmail);
                return newEmail.Id;
            }
            return emailInBase.Id;
        }

        public async Task<int> UpdateAsync(UpdateLocalMonitoredEmailsRequest request)
        {
            if (request.Id < 0) return 400;
            Action<LocalMonitoredEmails> updateDelegate = delegate (LocalMonitoredEmails e)
            {
                string newEmail = request.Email ?? e.Email;
                bool isAllowed = request.IsReplyAllowed ?? e.IsReplyAllowed;
                e.Email = newEmail.Trim();
                e.IsReplyAllowed = isAllowed;
            };
            bool res = false;
            try
            {
                res = await dbservice.UpdateAsync(request.Id, updateDelegate);
            }
            catch
            {
                return 400;
            }
            return res ? 200 : 404;
        }
        public async Task<bool> IsEmailExists(string email)
        {
            return await dbservice.FindAsync<LocalMonitoredEmails>(e => e.Email == email) == null ? false : true;
        }
        public async Task<bool> IsReplyAllowed(string email)
        {
            var entity = await dbservice.FindAsync<LocalMonitoredEmails>(e => e.Email == email);
            if (entity == null) return false;
            return entity.IsReplyAllowed;
        }
    }
}
