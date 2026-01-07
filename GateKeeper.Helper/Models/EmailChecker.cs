using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGPGK.Models
{
    internal class EmailChecker
    {
        private readonly AppSettings _appSettings;
        private bool _isUpdateAllowed = true;
        private FileDataStore fileDataStore;
        public bool isUpdateAllowed {  get { return _isUpdateAllowed; } }

        public EmailChecker(AppSettings appSettings, FileDataStore filedatastore)
        {
            _appSettings = appSettings;
            this.fileDataStore = filedatastore;
        }
        public void DisableUpdates()
        {
            _isUpdateAllowed = false;
        }
        public void EnableUpdates()
        {
            _isUpdateAllowed = true;
        }

        public bool isAdressMonitored(string emailAddress)
        {
            return fileDataStore.monitoredSenders.Contains(emailAddress);
        }
        public bool isSenderReplyAllowed(string emaiAddress)
        {
            return fileDataStore.replyAllowedSenders.Contains(emaiAddress);
        }
        public bool isRecipientReplyAllowed(string? emailAddress)
        {
            if (emailAddress == null) return false;
            return fileDataStore.replyAllowedRecipients.Contains(emailAddress);
        }
        public bool isAddressNotAllowed(string? recipient)
        {
            if (recipient == null) return false;
            recipient = recipient.Trim();
            string domain = recipient.Substring(recipient.IndexOf('@')+1);
            return !fileDataStore.allowedDomains.Contains(domain) && !fileDataStore.allowedRecipients.Contains(recipient);
        }
        public void UpdateStore(FileTypes fileType, List<string> data)
        {
            fileDataStore.UpdateStore(fileType, data);
        }
        public void AddReplyAllowedRecipient(string emailAddress)
        {
            fileDataStore.AddReplyAllowedRecipient(emailAddress);
        }
    }
}
