using CGPGK.Models;
using FluentFTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGPGK.Utils
{
    internal static class FTP
    {
        private static FtpDataProvider? instance = null;

        public static FtpDataProvider? GetInstance()
        {
            return instance;
        }
        public static FtpDataProvider GetInstance(AppSettings appSettings)
        {
            if (instance == null)
            {
                instance = new FtpDataProvider(appSettings);
            }
            return instance;
        }
    }
    internal class FtpDataProvider
    {
        private readonly AppSettings _appSettings;
        private CancellationToken updateSourceToken = new CancellationTokenSource().Token;
        public FtpDataProvider(AppSettings appSettings)
        {
            _appSettings = appSettings;
        }
        public async Task<MonitoredFile?> FileInfo(MonitoredFile file) 
        {
            MonitoredFile result = new(file.FullName, file.FileType);
            var connectionSettings = _appSettings.ConnectionSettings;
            try
            {
                using (var ftp = new AsyncFtpClient(connectionSettings.host, connectionSettings.login, connectionSettings.password))
                {
                    await ftp.Connect(updateSourceToken);
                    var items = await ftp.GetListing("/");
                    foreach (var item in items)
                    {
                        if (file.FullName == item.FullName)
                        {
                            DateTime time = await ftp.GetModifiedTime(item.FullName);
                            result = new(file.FullName, file.FileType, time);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("* Unable to connect to ftp");
                Console.Error.WriteLine(e.Message);
                return null;
            }
            return result;
        }
        public async Task<HashSet<string>> DownloadFileFromFTPAsync(string fileName)
        {
            var connectionSettings = _appSettings.ConnectionSettings;
            MemoryStream? stream = new MemoryStream();
            HashSet<string> result = new();
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
                                line = line.Trim().ToLowerInvariant();
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
