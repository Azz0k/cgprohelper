using CGPGK.Models;
using GateKeeper.Core.Application;
using GateKeeper.Core.Models.ApiModels;
using GateKeeper.Helper.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using static CGPGK.Utils.Utils;
using static GateKeeper.Core.Utils.Utils;

namespace GateKeeper.Helper.Application
{
    internal class HelperApplication
    {
        private AllowedEmailsApplication allowedEmailsApplication;
        private AllowedDomainsApplication allowedDomainsApplication;
        private ForeingEmailsApplication foreingEmailsApplication;
        private LocalMonitoredEmailsApplication localMonitoredEmailsApplication;
        private BlockedEmailsApplication blockedEmailApplication;
        private readonly AppSettings appSettings;
        public HelperApplication(
            AllowedEmailsApplication emailsApplication,
            AllowedDomainsApplication domainsApplication,
            ForeingEmailsApplication foreingEmailsApplication,
            LocalMonitoredEmailsApplication localMonitoredEmailsApplication,
            BlockedEmailsApplication blockedEmailApplication,
            AppSettings appSettings)
        {
            this.appSettings = appSettings;
            this.allowedEmailsApplication = emailsApplication;
            this.allowedDomainsApplication = domainsApplication;
            this.foreingEmailsApplication = foreingEmailsApplication;
            this.localMonitoredEmailsApplication = localMonitoredEmailsApplication;
            this.blockedEmailApplication = blockedEmailApplication;
        }
        public async Task UpdateEmailsFromFTPAsync( HashSet<string> data)
        {
            await allowedEmailsApplication.SyncTable(data);
        }
        public async Task UpdateDomainsFromFTPAsync(HashSet<string> data)
        {
            await allowedDomainsApplication.SyncTable(data);
        }
        public async Task ProcessMessageAsync(string message)
        {
            string[] messageParts = message.Split();
            string lineNumber = messageParts[0];
            string command = messageParts[1].ToLowerInvariant();
            switch (command)
            {
                case "quit":
                    PrintGoodMessage(lineNumber);
                    Environment.Exit(0);
                    break;
                case "intf":
                    Print($"{lineNumber} INTF 3");
                    break;
                case "file":
                    if (messageParts.Length != 3)
                    {
                        PrintGoodMessage(lineNumber);
                        PrintLogMessage("Error: wrong INTF format!");
                        return;
                    }
                    string fileName = messageParts[2];
                    var file = Path.Combine(appSettings.baseDir, fileName);
                    EmailFields? emailFields = await ParseEmailFile(file);
                    if (emailFields == null)
                    {
                        PrintGoodMessage(lineNumber);
                        PrintLogMessage($"{file} file does not exists or is corrupted");
                        return;
                    }
                    if (await EnsureSendingAllowed(emailFields))
                    {
                        PrintGoodMessage(lineNumber);
                    }
                    else
                    {
                        PrintBadMessage(lineNumber);
                    }
                    break;
                default:
                    PrintGoodMessage(lineNumber);
                    PrintLogMessage($"Error: command {command} is not implemented");
                    break;
            }
        }
        internal async Task<EmailFields?> ParseEmailFile(string fileName)
        {
            EmailFields? result = null;
            if (!EnsureFileExists(fileName)) return result;
            using (FileStream fs = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (BufferedStream bs = new BufferedStream(fs))
            using (StreamReader sr = new StreamReader(bs))
            {
                string? line;
                while ((line = sr.ReadLine()) != null && line!="")
                {
                    string? sender = GetSender(line);
                    if (sender != null)
                    {
                        result = new(sender);
                    }
                    string? recipient = GetRecipient(line);
                    if (recipient != null)
                    {
                        if (result == null) return result;
                        result.To.Add(recipient);
                    }    
                }
                return result;
            }
        }
        internal async Task<bool> EnsureSendingAllowed(EmailFields emailFields)
        {
            string domain;
            bool isEmailMonitored = await localMonitoredEmailsApplication.IsEmailExists(emailFields.From);
            if (isEmailMonitored)
            {
                bool isReplyAllowed = await localMonitoredEmailsApplication.IsReplyAllowed(emailFields.From);
                foreach (string recipient in emailFields.To)
                {
                    if (isReplyAllowed)
                    {
                        if (await foreingEmailsApplication.IsEmailExists(recipient))
                            continue;
                    }

                    domain = recipient.Substring(recipient.IndexOf('@') + 1);
                    if (await allowedEmailsApplication.IsEmailExists(recipient)) continue;
                    if (await allowedDomainsApplication.IsDomainExists(domain)) continue;
                    var request = new AddBlockedEmailRequest() {
                        SenderEmail = emailFields.From, 
                        RecipientEmail = recipient, 
                        Date = GenerateReceivedDate(), 
                        Time = GenerateReceivedTime()
                    };
                    await blockedEmailApplication.AddAsync(request);
                    return false;
                }
                return true;
            }
            domain = emailFields.From.Substring(emailFields.From.IndexOf('@') + 1);
            if (await allowedEmailsApplication.IsEmailExists(emailFields.From)) return true;
            if (await allowedDomainsApplication.IsDomainExists(domain)) return true;
            foreach (string recipient in emailFields.To)
            {
                if (await localMonitoredEmailsApplication.IsReplyAllowed(recipient))
                {

                    await foreingEmailsApplication.AddAsync(new Core.Models.ApiModels.AddForeingEmailRequest() { Email = emailFields.From });
                    break;
                }
            }
            return true;
        }
    }
}
