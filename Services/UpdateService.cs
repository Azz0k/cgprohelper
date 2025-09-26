using CGProToCCAddressHelper.Models;
using FluentFTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace CGProToCCAddressHelper.Services
{
    internal class UpdateService
    {
        private AllowedRecipients _allowedRecipients;
        private readonly AppSettings _appSettings;
        private string recipientsFile;
        private CancellationTokenSource updateSource = new CancellationTokenSource();
        private FtpService _ftpService;
        private FTPFile emailsFullListFile = new FTPFile();
        public UpdateService(AppSettings appSettings, AllowedRecipients allowedRecipients, FtpService ftpService) 
        {
            _appSettings = appSettings;
            _allowedRecipients = allowedRecipients;
            string currentDir = _appSettings.currentDir;
            string fileName = _appSettings.emailsLocalFullFileName;
            recipientsFile = Path.Combine(currentDir, fileName);
            _ftpService = ftpService;
        }

        public async Task UpdateDataFirstTime()
        {
            await _ftpService.DownloadFullBaseIfNeededAsync();
            ReadRecipientsFromFile();
            updateSource = new CancellationTokenSource();
            var backgroundTask = Task.Run(() => {BackGroundLoop(); }, updateSource.Token);
        }
        private async Task BackGroundLoop()
        {
            while (!updateSource.Token.IsCancellationRequested)
            {
                await Task.Delay(1000*10, updateSource.Token);
                await _ftpService.DownloadFullBaseIfNeededAsync();
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
