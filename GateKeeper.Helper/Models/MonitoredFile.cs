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
    internal class MonitoredFileOnFTP : MonitoredFile
    {
        [SetsRequiredMembers]
        public MonitoredFileOnFTP(string fullName, FileTypes fileType) : base(fullName, fileType)
        {
        }
        public async Task<bool> CheckFileAsync()
        {
            var ftp = FTP.GetInstance();
            if (ftp == null) return false;
            var file = await ftp.FileInfo(this);
            if (file == null)  return false;
            if (this.modifiedTime == file?.ModifiedTime)
                return false;
            return true;
        }
        public async Task SaveNewTimeAsync()
        {
            var ftp = FTP.GetInstance();
            if (ftp == null) return;
            var file = await ftp.FileInfo(this);
            modifiedTime = file?.ModifiedTime ?? modifiedTime;
        }
        public async Task<HashSet<string>> ReadAllLinesAsync()
        {
            HashSet<string> lines = new(StringComparer.OrdinalIgnoreCase);
            var ftp = FTP.GetInstance();
            if (ftp != null)
                lines = await ftp.DownloadFileFromFTPAsync(this.FullName);
            await SaveNewTimeAsync();
            return lines;
        }
    }
}
