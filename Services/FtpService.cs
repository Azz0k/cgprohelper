using CGProToCCAddressHelper.Models;
using FluentFTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace CGProToCCAddressHelper.Services
{
    internal class FtpService
    {
        private readonly AppSettings _appSettings;
        private string recipientsFile;
        private CancellationTokenSource updateSource = new CancellationTokenSource();
        private FTPFile emailsFullListFile = new FTPFile();
        private bool needUpdateEmailsFullListFile = false;
        
        
        public FtpService(AppSettings appSettings) 
        {
            _appSettings = appSettings;
            string currentDir = _appSettings.currentDir;
            string fileName = _appSettings.emailsLocalFullFileName;
            recipientsFile = Path.Combine(currentDir, fileName);
            emailsFullListFile.fullName = _appSettings.ConnectionSettings.emailsFullFileName;
        }
        public async Task DownloadFullBaseIfNeededAsync()
        {
            await ChekIsThereDifferentFullEmailsFileOnFTP();
            if (needUpdateEmailsFullListFile)
            {
                await DownloadFullBaseAsync();
                needUpdateEmailsFullListFile = false;
            }

        }
        private async Task ExecuteAsync(Func<AsyncFtpClient, Task> operation)
        {
            var connectionSettings = _appSettings.ConnectionSettings;
            try
            {
                using (var ftp = new AsyncFtpClient(connectionSettings.host, connectionSettings.login, connectionSettings.password))
                {
                    await ftp.Connect(updateSource.Token);
                    await operation(ftp);
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("* Unable to connect to ftp");
                Console.Error.WriteLine(e.Message);
            }
        }
        private async Task ChekIsThereDifferentFullEmailsFileOnFTP()
        {
            needUpdateEmailsFullListFile = false;
            await ExecuteAsync(async (ftp) =>
            {
                var items = await ftp.GetListing("/");
                foreach (var item in items)
                {
                    if (item.FullName == emailsFullListFile.fullName)
                    {
                        var size = await ftp.GetFileSize(item.FullName);
                        var time = await ftp.GetModifiedTime(item.FullName);
                        if (size!=emailsFullListFile.size && time != emailsFullListFile.modifiedTime)
                        {
                            emailsFullListFile.size = size;
                            emailsFullListFile.modifiedTime = time;
                            needUpdateEmailsFullListFile = true;
                        }
                    }
                    
                }

            });
            
        }
        private async Task DownloadFullBaseAsync()
        {
            await ExecuteAsync(async (ftp) =>
            {
                await ftp.DownloadFile(recipientsFile, _appSettings.ConnectionSettings.emailsFullFileName, FtpLocalExists.Overwrite, FtpVerify.Retry, token: updateSource.Token);
            });
        }

    }
}
