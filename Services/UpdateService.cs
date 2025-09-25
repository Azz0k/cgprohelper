using CGProToCCAddressHelper.Models;
using FluentFTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGProToCCAddressHelper.Services
{
    internal class UpdateService
    {
        private AllowedRecipients _allowedRecipients;
        private readonly AppSettings _appSettings;
        private string recipientsFile;
        public UpdateService(AppSettings appSettings, AllowedRecipients allowedRecipients) 
        {
            _appSettings = appSettings;
            _allowedRecipients = allowedRecipients;
            string currentDir = _appSettings.currentDir;
            string fileName = _appSettings.emailsLocalFullFileName;
            recipientsFile = Path.Combine(currentDir, fileName);
        }

        public async Task UpdateData()
        {
            await DownloadFullBaseAsync();
            ReadRecipientsFromFile();
        }
        private async Task DownloadFullBaseAsync()
        {
            var connectionSettings = _appSettings.ConnectionSettings;
            var token = new CancellationToken();
            try
            {
                using (var ftp = new AsyncFtpClient(connectionSettings.host, connectionSettings.login, connectionSettings.password))
                {
                    await ftp.Connect(token);
                    await ftp.DownloadFile(recipientsFile, connectionSettings.emailsFullFileName, FtpLocalExists.Overwrite, FtpVerify.Retry, token: token);
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("* Unable to download file from ftp");
                Console.Error.WriteLine(e.Message);
            }
            
        }
        private void ReadRecipientsFromFile()
        {
            _allowedRecipients.ClearPatients();
            try
            {
                using (FileStream fs = File.Open(recipientsFile, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (BufferedStream bs = new BufferedStream(fs))
                using (StreamReader sr = new StreamReader(bs))
                {
                    string? line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        _allowedRecipients.Add(line);
                    }

                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("* Error: The address files not found");
                Console.Error.WriteLine(e.Message);
                Environment.Exit(1);
            }
        }
    }
}
