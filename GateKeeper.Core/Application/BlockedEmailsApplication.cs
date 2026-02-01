using GateKeeper.Core.Abstracts;
using GateKeeper.Core.Interfaces;
using GateKeeper.Core.Models.ApiModels;
using GateKeeper.Core.Models.Entities;
using GateKeeper.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace GateKeeper.Core.Application
{
    public class BlockedEmailsApplication: ApplicationBase
    {
        private IDataBaseService dbservice;
        public BlockedEmailsApplication(DatabaseService dbservice) : base(dbservice)
        {
            this.dbservice = dbservice;
        }
        public async Task<int> AddAsync(AddBlockedEmailRequest request)
        {
            BlockedEmails newEmail =  new BlockedEmails()
            {
                SenderEmail = request.SenderEmail,
                RecipientEmail = request.RecipientEmail,
                Date = request.Date,
                Time = request.Time
            };
            newEmail = await dbservice.CreateAsync(newEmail);
            return newEmail.Id;
        }
    }
}
