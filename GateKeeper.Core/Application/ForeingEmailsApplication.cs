using GateKeeper.Core.Abstracts;
using GateKeeper.Core.Interfaces;
using GateKeeper.Core.Models.ApiModels;
using GateKeeper.Core.Models.Entities;
using GateKeeper.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;
using static GateKeeper.Core.Utils.Utils;

namespace GateKeeper.Core.Application
{
    public class ForeingEmailsApplication: ApplicationBase
    {
        private IDataBaseService dbservice;
        public ForeingEmailsApplication(DatabaseService dbservice) : base(dbservice)
        {
            this.dbservice = dbservice;
        }
        public async Task<int> AddAsync(AddForeingEmailRequest foreingEmailRequest)
        {
            if (foreingEmailRequest.Email == null) return -1;
            string email = foreingEmailRequest.Email.Trim();
            ForeingEmails? foreingEmailInBase = await dbservice.FindAsync<ForeingEmails>(d => d.Email == email);
            if (foreingEmailInBase == null)
            {
                ForeingEmails newEmail = new ForeingEmails() { Email = foreingEmailRequest.Email, ReceivedDate = GenerateReceivedDate() };
                newEmail = await dbservice.CreateAsync(newEmail);
                return newEmail.Id;
            }
            await dbservice.UpdateAsync(foreingEmailInBase.Id, (ForeingEmails email) => email.ReceivedDate = GenerateReceivedDate());
            return foreingEmailInBase.Id;
        }
        public async Task RemoveDeprecatedRecordsAsync()
        {
            string deprecatedDate = GenerateDeprecatedDate();
            List<ForeingEmails> list = await GetAllRecordsAsync<ForeingEmails>();
            foreach (ForeingEmails email in list)
            {
                if (string.Compare(email.ReceivedDate,deprecatedDate)>=0)
                {
                    await dbservice.DeleteAsync<ForeingEmails>(email.Id)    ;
                }
            }

        }
        public async Task<bool> IsEmailExists(string email)
        {
            return await dbservice.FindAsync<ForeingEmails>(e => e.Email == email) == null ? false : true;
        }

    }
}
