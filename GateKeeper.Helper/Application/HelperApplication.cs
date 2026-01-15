using CGPGK.Models;
using GateKeeper.Core.Application;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace GateKeeper.Helper.Application
{
    internal class HelperApplication
    {
        private AllowedEmailsApplication emailsApplication;
        private AllowedDomainsApplication domainsApplication;
        private ForeingEmailsApplication foreingEmailsApplication;
        private LocalMonitoredEmailsApplication localMonitoredEmailsApplication;
        private readonly Dictionary<FileTypes, Func<List<string>, Task>> updateHandlers;
        public HelperApplication(
            AllowedEmailsApplication emailsApplication, 
            AllowedDomainsApplication domainsApplication, 
            ForeingEmailsApplication foreingEmailsApplication, 
            LocalMonitoredEmailsApplication localMonitoredEmailsApplication)
        {
            this.emailsApplication = emailsApplication;
            this.domainsApplication = domainsApplication;
            this.foreingEmailsApplication = foreingEmailsApplication;
            this.localMonitoredEmailsApplication = localMonitoredEmailsApplication;
            updateHandlers = new Dictionary<FileTypes, Func<List<string>, Task>>
            {
                {
                    FileTypes.EmailsFull,
                    async data =>
                    {
                        await emailsApplication.SyncTable(data);
                    }
                },
                {
                    FileTypes.EmailsDiff,
                    async data =>
                    {
                        await emailsApplication.AddAsync(data);
                    }
                },
                {
                    FileTypes.DomainsFull,
                    async data =>
                    {
                        await domainsApplication.SyncTable(data);
                    }
                },
                {
                    FileTypes.DomainsDiff,
                    async data =>
                    {
                        await domainsApplication.AddAsync(data);
                    }
                }
            };
        }
        public async Task UpdateDataFromFTPAsync(FileTypes fileType, List<string> data)
        {
            if (updateHandlers.TryGetValue(fileType, out var handler))
            {
                await handler(data);
            }
        }

    }
}
