using CGProToCCAddressHelper.Utils;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static CGProToCCAddressHelper.Utils.Utils;


namespace CGProToCCAddressHelper.Models
{
    internal interface ICheckUpdate
    {
        public Task<List<string>> ReadAllLinesIfChangedAsync();
        public Task SaveNewTimeAsync(HashSet<string> data);

    }
    enum FileTypes { EmailsFull, EmailsDiff, DomainsFull, DomainsDiff, monitoredSenders, relplyAllowedSenders, replyAllowedRecipients };
    internal class MonitoredFile:IComparable
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

        public int CompareTo(object? obj)
        {
            return modifiedTime.CompareTo(obj);
        }
    }

    internal class MonitoredFileOnDisk : MonitoredFile, ICheckUpdate
    {
        [SetsRequiredMembers]
        public MonitoredFileOnDisk(string fullName, FileTypes fileType) : base(fullName, fileType)
        {
        }
        public async Task<bool> CheckFileAsync()
        {
            bool isChanged = false;
            var file = new FileInfo(FullName);
            if (!file.Exists)
            {
                file.Create().Close();
                modifiedTime = file.LastWriteTime;
                return isChanged;
            }
            if (modifiedTime != file.LastWriteTime)
                isChanged = true;
            return isChanged;
        }
        public async Task SaveNewTimeAsync(HashSet<string> data)
        {
            var file = new FileInfo(FullName);
            modifiedTime = file.LastWriteTime;
        }
        public async Task<List<string>> ReadAllLinesIfChangedAsync()
        {
            var lines = new List<string>();
            bool isChanged = await CheckFileAsync();
            if (!isChanged) return lines;
            using (FileStream fs = File.Open(FullName, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (BufferedStream bs = new BufferedStream(fs))
            using (StreamReader sr = new StreamReader(bs))
            {
                string? line;
                while ((line = sr.ReadLine()) != null)
                {
                    line = line.Trim();
                    if (line != "") lines.Add(line);
                }
            }
            return lines;
        }
    }
    internal class MonitoredFileOnFTP : MonitoredFile, ICheckUpdate
    {
        [SetsRequiredMembers]
        public MonitoredFileOnFTP(string fullName, FileTypes fileType) : base(fullName, fileType)
        {
        }
        public async Task<bool> CheckFileAsync()
        {
            bool isChanged = false;
            var ftp = FTP.GetInstance();
            if (ftp == null) return false;
            var file = await ftp.FileInfo(this);
            if (this.modifiedTime != file?.ModifiedTime)
                isChanged = true;
            return isChanged;
        }
        public async Task SaveNewTimeAsync(HashSet<string> data)
        {
            var ftp = FTP.GetInstance();
            if (ftp == null) return;
            var file = await ftp.FileInfo(this);
            modifiedTime = file?.ModifiedTime ?? modifiedTime;
        }
        public async Task<List<string>> ReadAllLinesIfChangedAsync()
        {
            var lines = new List<string>();
            bool isChanged = await CheckFileAsync();
            if (!isChanged) return lines;
            var ftp = FTP.GetInstance();
            if (ftp != null)
                lines = await ftp.DownloadFileFromFTPAsync(this.FullName);
            return lines;
        }
    }
    internal class MonitoredSQLite : MonitoredFile, ICheckUpdate
    {

        [SetsRequiredMembers]
        public MonitoredSQLite(string fullName, FileTypes fileType) : base(fullName, fileType)
        {
        }

        public async Task<List<string>> ReadAllLinesIfChangedAsync()
        {
            //await DropTableAsync();
            var lines = new List<string>();
            lines.Add("postmaster@postmaster");
            using SqliteConnection connection = new($"Data Source={FullName}");
            connection.Open();
            SqliteCommand command = new();
            command.Connection = connection;
            command.CommandText = "CREATE TABLE IF NOT EXISTS AllowedEmails (Email TEXT UNIQUE, LastReplyDate TEXT);";
            command.ExecuteNonQuery();
            command.CommandText = "SELECT * FROM AllowedEmails";
            using SqliteDataReader reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    string email = reader.GetString(0);
                    string date = reader.GetString(1);
                    lines.Add(email);   
                }
            }
            return lines;
        }

        private async Task DropTableAsync()
        {
            using SqliteConnection connection = new($"Data Source={FullName}");
            connection.Open();
            SqliteCommand command = new();
            command.Connection = connection;
            command.CommandText = "DROP TABLE AllowedEmails;";
            command.ExecuteNonQuery();
        }
        private async Task CheckDatesAndDeleteFromSet(HashSet<string> data)
        {
            string deadLine = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
            using SqliteConnection connection = new($"Data Source={FullName}");
            connection.Open();
            SqliteCommand command = new();
            command.Connection = connection;
            command.CommandText = $"SELECT * FROM AllowedEmails WHERE LastReplyDate<\"{deadLine}\";";
            HashSet<string> lines = new(StringComparer.OrdinalIgnoreCase);
            using SqliteDataReader reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    string email = reader.GetString(0);
                    string date = reader.GetString(1);
                    lines.Add(email);
                }
            }
            data = data.Except(lines).ToHashSet<string>(StringComparer.OrdinalIgnoreCase);
        }
        public async Task SaveNewTimeAsync(HashSet<string> data)
        {
            var lines = await ReadAllLinesIfChangedAsync();
            await CheckDatesAndDeleteFromSet(data);
            var toDelete = lines.Except(data).ToList();
            var toAdd = data.Except(lines).ToList();
            using SqliteConnection connection = new($"Data Source={FullName}");
            connection.Open();
            SqliteCommand command = new();
            command.Connection = connection;
            command.CommandText = "DELETE FROM AllowedEmails WHERE Email = @email";
            foreach (var email in toDelete)
            {
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@email", email);
                command.ExecuteNonQuery();
            }
            command.CommandText = "INSERT INTO AllowedEmails (Email, LastReplyDate) VALUES (@email, @date)";
            foreach (var email in toAdd)
            {
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@email", email);
                command.Parameters.AddWithValue("@date", DateTime.Now.ToString("yyyy-MM-dd"));
                command.ExecuteNonQuery();
            }
        }
    }


    internal class MonitoredFiles
    {
        private Dictionary<FileTypes,ICheckUpdate> _files = new();
        private EmailChecker emailChecker;
        private AppSettings appSettings;
        private FileDataStore fileDataStore;
        public MonitoredFiles(AppSettings appsettings, EmailChecker emailChecker, FileDataStore store)
        {
            this.appSettings = appsettings;
            this.emailChecker = emailChecker;
            fileDataStore = store;
            _files.Add(FileTypes.EmailsFull, new MonitoredFileOnFTP(appsettings.ConnectionSettings.emailsFullFileName, FileTypes.EmailsFull));
            _files.Add(FileTypes.EmailsDiff, new MonitoredFileOnFTP(appsettings.ConnectionSettings.emailsDiffFileName, FileTypes.EmailsDiff));
            _files.Add(FileTypes.DomainsFull, new MonitoredFileOnFTP(appsettings.ConnectionSettings.domainsFullFileName, FileTypes.DomainsFull));
            _files.Add(FileTypes.DomainsDiff, new MonitoredFileOnFTP(appsettings.ConnectionSettings.domainsDiffFileName, FileTypes.DomainsDiff));
            _files.Add(FileTypes.relplyAllowedSenders, new MonitoredFileOnDisk(PathCombine(appsettings.replyAllowedSendersFileName), FileTypes.relplyAllowedSenders));
            _files.Add(FileTypes.replyAllowedRecipients, new MonitoredSQLite(PathCombine(appSettings.replyAllowedRecipientsFileName), FileTypes.replyAllowedRecipients));
            _files.Add(FileTypes.monitoredSenders, new MonitoredFileOnDisk(PathCombine(appSettings.monitoredSendersFileName), FileTypes.monitoredSenders));
        }
        private string PathCombine(string fileName)
        {
            return Path.Combine(appSettings.currentDir, fileName);
        }
        public async Task CheckAllFilesAsync()
        {
            foreach (var pairs in _files)
            {
                FileTypes fileType = pairs.Key;
                ICheckUpdate file = pairs.Value;
                List<string> data = await file.ReadAllLinesIfChangedAsync();
                if (data.Count>0 && emailChecker.isUpdateAllowed) 
                {
                    emailChecker.UpdateStore(fileType, data);
                    if (fileType!=FileTypes.replyAllowedRecipients) PrintLogMessage($"data from {fileType.ToString()} updated");
                    await file.SaveNewTimeAsync(fileDataStore.sets[fileType]);
                }




            }
        }

    }

}
