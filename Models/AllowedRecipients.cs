using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGProToCCAddressHelper.Models
{
    internal class AllowedRecipients
    {
        private readonly AppSettings _appSettings;
        private HashSet<string> allowedRecipients = new HashSet<string>();
        private HashSet<string> allowedDomains;
        private bool _isUpdateAllowed = true;
        public bool isUpdateAllowed {  get { return _isUpdateAllowed; } }

        public AllowedRecipients(AppSettings appSettings)
        {
            _appSettings = appSettings;
            allowedDomains = new HashSet<string>(appSettings.allowedDomains);
        }

        public void DisableUpdates()
        {
            _isUpdateAllowed = false;
        }
        public void EnableUpdates()
        {
            _isUpdateAllowed = true;
        }
        public void ClearPatients()
        {
            allowedRecipients.Clear();
        }
        public void UpdateDomains(string[] domains)
        {
            allowedDomains = new HashSet<string>(domains);
        }
        public void Add (string recipient)
        {
            allowedRecipients.Add(recipient.Trim());
        }
        public void Update(List<string> recipients)
        {
            allowedRecipients = new HashSet<string>(recipients);
        }

        public bool isAddressNotAllowed(string recipient)
        {
            string domain = recipient.Substring(recipient.IndexOf('@'));
            return !allowedDomains.Contains(domain) && !allowedRecipients.Contains(recipient);
        }
    }
}
