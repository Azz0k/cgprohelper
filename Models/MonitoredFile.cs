using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGProToCCAddressHelper.Models
{
    enum FileTypes { EmailsFull, EmailsDiff, DomainsFull, DomainsDiff };
    internal class MonitoredFile
    {
        public string FullName { get; set; } = string.Empty;
        public long Size { get; set; } = 0;
        public DateTime ModifiedTime { get; set; } = DateTime.MinValue;
        public bool IsChanged { get; set; } = false;
        public required FileTypes FileType { get; set; }

        [SetsRequiredMembers]
        public MonitoredFile(string fullName, FileTypes fileType)
        {
            FullName = fullName;
            FileType = fileType;
        }
    }
    internal class UpdatesFromFile
    {
        public required FileTypes FileType;
        public required HashSet<string> Data { get; set; }
    }

}
