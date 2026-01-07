using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGPGK.Models
{
    internal class FileDataStore
    {
        public Dictionary<FileTypes,HashSet<string>> sets = new();
        private HashSet<string> _allowedRecipients = new (StringComparer.OrdinalIgnoreCase);
        public HashSet<string> allowedRecipients { get { return _allowedRecipients; } }
        private HashSet<string> _allowedDomains = new(StringComparer.OrdinalIgnoreCase);
        public HashSet<string> allowedDomains { get { return _allowedDomains; } }
        private readonly Dictionary<FileTypes, Action<List<string>>> updateHandlers;
        private HashSet<string> _monitoredSenders = new(StringComparer.OrdinalIgnoreCase);
        public HashSet<string> monitoredSenders { get { return _monitoredSenders; } }
        private HashSet<string> _replyAllowedSenders = new(StringComparer.OrdinalIgnoreCase);
        public HashSet<string> replyAllowedSenders { get { return _replyAllowedSenders; } }
        private HashSet<string> _replyAllowedRecipients = new(StringComparer.OrdinalIgnoreCase);
        public HashSet<string> replyAllowedRecipients { get { return _replyAllowedRecipients; } }
        public FileDataStore(AppSettings appSettings)
        {
            sets.Add(FileTypes.EmailsFull, _allowedRecipients);
            sets.Add(FileTypes.EmailsDiff, _allowedRecipients);
            sets.Add(FileTypes.DomainsFull, _allowedDomains);
            sets.Add(FileTypes.DomainsDiff, _allowedDomains);
            sets.Add(FileTypes.monitoredSenders, _monitoredSenders);
            sets.Add(FileTypes.relplyAllowedSenders, _replyAllowedSenders);
            sets.Add(FileTypes.replyAllowedRecipients, _replyAllowedRecipients);
            updateHandlers = new Dictionary<FileTypes, Action<List<string>>>
            {
                { FileTypes.EmailsFull, data =>
                    {
                        _allowedRecipients.Clear();
                        _allowedRecipients.UnionWith(data); 
                    }
                },
                { FileTypes.EmailsDiff, data =>
                    {
                        _allowedRecipients.UnionWith(data);
                    }
                },
                { FileTypes.DomainsFull, data =>
                    {
                        _allowedDomains.Clear();
                        _allowedDomains.UnionWith(appSettings.allowedDomains);
                        _allowedDomains.UnionWith(data);
                    }
                },
                { FileTypes.DomainsDiff, data =>
                    {
                        _allowedDomains.UnionWith(data);
                    }
                },
                { FileTypes.monitoredSenders, data =>
                    {
                        _monitoredSenders.Clear();
                        _monitoredSenders.UnionWith(data);
                    }
                },
                { FileTypes.relplyAllowedSenders, data =>
                    {
                        _replyAllowedSenders.Clear();
                        _replyAllowedSenders.UnionWith(data);
                    }
                },
                { FileTypes.replyAllowedRecipients, data =>
                    {
                        //_replyAllowedRecipients.Clear();
                        _replyAllowedRecipients.UnionWith(data);
                    }
                }
            };
        }
        public void UpdateStore(FileTypes fileType, List<string> data)
        {

            if (updateHandlers.TryGetValue(fileType, out var handler))
            {
                handler(data);
            }
        }
        public void AddReplyAllowedRecipient(string email)
        {
            _replyAllowedRecipients.Add(email);
        }
    }
}
