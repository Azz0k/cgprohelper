using CGProToCCAddressHelper.Models;
using FluentFTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace CGProToCCAddressHelper.Services
{
    internal class FtpService
    {
        private readonly AppSettings _appSettings;
        private CancellationToken updateSourceToken = new CancellationTokenSource().Token;
        private Dictionary<string,MonitoredFile> monitoredFiles = new Dictionary<string,MonitoredFile>();
        public FtpService(AppSettings appSettings) 
        {
            _appSettings = appSettings;
            monitoredFiles.Add(_appSettings.ConnectionSettings.emailsFullFileName, new MonitoredFile(_appSettings.ConnectionSettings.emailsFullFileName, FileTypes.EmailsFull));
            monitoredFiles.Add(_appSettings.ConnectionSettings.emailsDiffFileName, new MonitoredFile(_appSettings.ConnectionSettings.emailsDiffFileName, FileTypes.EmailsDiff));
            monitoredFiles.Add(_appSettings.ConnectionSettings.domainsFullFileName, new MonitoredFile(_appSettings.ConnectionSettings.domainsFullFileName, FileTypes.DomainsFull));
            monitoredFiles.Add(_appSettings.ConnectionSettings.domainsDiffFileName, new MonitoredFile(_appSettings.ConnectionSettings.domainsDiffFileName, FileTypes.DomainsDiff));
        }
        public async Task<List<UpdatesFromFile>> DownloadIfNeededAsync(CancellationToken token)
        {
            List<UpdatesFromFile> result = new List<UpdatesFromFile>();
            updateSourceToken = token;
            await CheckIsThereDifferentFilesOnFTP();
            foreach (var file in monitoredFiles.Values)
            {
                if (file.IsChanged)
                {
                    var data = await DownloadFileFromFTPAsync(file.FullName);
                    result.Add(new UpdatesFromFile() { Data = data, FileType = file.FileType });
                    file.IsChanged = false;
                }
            }
            return result;
        }
        private async Task ExecuteAsync(Func<AsyncFtpClient, Task> operation)
        {
            var connectionSettings = _appSettings.ConnectionSettings;
            try
            {
                using (var ftp = new AsyncFtpClient(connectionSettings.host, connectionSettings.login, connectionSettings.password))
                {
                    await ftp.Connect(updateSourceToken);
                    await operation(ftp);
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("* Unable to connect to ftp");
                Console.Error.WriteLine(e.Message);
            }
        }

        private async Task CheckIsThereDifferentFilesOnFTP()
        {

            await ExecuteAsync(async (ftp) =>
            {
                var items = await ftp.GetListing("/");
                foreach (var item in items)
                {
                    if (monitoredFiles.ContainsKey(item.FullName))
                    {
                        var size = await ftp.GetFileSize(item.FullName);
                        var time = await ftp.GetModifiedTime(item.FullName);
                        if (size!= monitoredFiles[item.FullName].Size || time != monitoredFiles[item.FullName].ModifiedTime)
                        {
                            monitoredFiles[item.FullName].Size = size;
                            monitoredFiles[item.FullName].ModifiedTime = time;
                            monitoredFiles[item.FullName].IsChanged = true;
                        }
                    }
                }
            });
            
        }
        private async Task<HashSet<string>> DownloadFileFromFTPAsync(string fileName)
        {
            var connectionSettings = _appSettings.ConnectionSettings;
            MemoryStream? stream = new MemoryStream();
            HashSet<string> result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            try
            {
                using (var ftp = new AsyncFtpClient(connectionSettings.host, connectionSettings.login, connectionSettings.password))
                {
                    await ftp.Connect(updateSourceToken);
                    await ftp.DownloadStream(stream, fileName, token: updateSourceToken);
                    if (stream != null)
                    {
                        stream.Position = 0;
                        using (StreamReader reader = new StreamReader(stream, Encoding.ASCII))
                        {
                            string? line;
                            while ((line = reader.ReadLine()) != null)
                            {
                                line = line.Trim();
                                if (line != "") 
                                {
                                    result.Add(line);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("* Unable to connect to ftp");
                Console.Error.WriteLine(e.Message);
            }
            finally
            {
                stream?.Dispose();
            }

            return result;

        }
    }
}
