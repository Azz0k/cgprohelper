using CGPGK.Utils;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static CGPGK.Utils.Utils;


namespace CGPGK.Models
{

    enum FileTypes { EmailsFull, EmailsDiff, DomainsFull, DomainsDiff};
    internal class MonitoredFile
    {
        public required string FullName { get; init; }
        protected DateTime modifiedTime { get; set; } = DateTime.MinValue;
        public DateTime ModifiedTime { get { return modifiedTime; } }
        public required FileTypes FileType { get; init; }

        [SetsRequiredMembers]
        public MonitoredFile(string fullName, FileTypes fileType)
        {
            FullName = fullName;
            FileType = fileType;
        }
        [SetsRequiredMembers]
        public MonitoredFile(string fullName, FileTypes fileType, DateTime time)
        {
            FullName = fullName;
            FileType = fileType;
            modifiedTime = time;
        }
    }
}
