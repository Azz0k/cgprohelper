using CGPGK.Models;
using GateKeeper.Core.Application;
using GateKeeper.Helper.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using static CGPGK.Utils.Utils;

namespace GateKeeper.Helper.Application
{
    internal class HelperApplication
    {
        private AllowedEmailsApplication emailsApplication;
        private AllowedDomainsApplication domainsApplication;
        private ForeingEmailsApplication foreingEmailsApplication;
        private LocalMonitoredEmailsApplication localMonitoredEmailsApplication;
        private readonly AppSettings appSettings;
        public HelperApplication(
            AllowedEmailsApplication emailsApplication,
            AllowedDomainsApplication domainsApplication,
            ForeingEmailsApplication foreingEmailsApplication,
            LocalMonitoredEmailsApplication localMonitoredEmailsApplication,
            AppSettings appSettings)
        {
            this.appSettings = appSettings;
            this.emailsApplication = emailsApplication;
            this.domainsApplication = domainsApplication;
            this.foreingEmailsApplication = foreingEmailsApplication;
            this.localMonitoredEmailsApplication = localMonitoredEmailsApplication;
        }
        public async Task UpdateEmailsFromFTPAsync( HashSet<string> data)
        {
            await emailsApplication.SyncTable(data);
        }
        public async Task UpdateDomainsFromFTPAsync(HashSet<string> data)
        {
            await domainsApplication.SyncTable(data);
        }
        public async Task ProcessMessageAsync(string message)
        {
            string[] messageParts = message.Split();
            string lineNumber = messageParts[0];
            string command = messageParts[1];
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
    }
}
