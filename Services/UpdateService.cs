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
        private int updateInterval = 60;
        public UpdateService(AppSettings appSettings, AllowedRecipients allowedRecipients, FtpService ftpService)
        {
            _appSettings = appSettings;
            _allowedRecipients = allowedRecipients;
            string currentDir = _appSettings.currentDir;
            string fileName = _appSettings.emailsLocalFullFileName;
            recipientsFile = Path.Combine(currentDir, fileName);
            _ftpService = ftpService;
            if (_appSettings.updateIntervalInSeconds > 0)
            {
                updateInterval = _appSettings.updateIntervalInSeconds;  
            }
        }

        public async Task UpdateDataFirstTime()
        {
            await _ftpService.DownloadFullBaseIfNeededAsync(updateSource.Token);
            ReadRecipientsFromFile();
            updateSource = new CancellationTokenSource();
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            var backgroundTask = Task.Run(() => { BackGroundLoop(); }, updateSource.Token);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }

        private async Task BackGroundLoop()
        {
            while (!updateSource.Token.IsCancellationRequested)
            {
                await Task.Delay(1000 * updateInterval, updateSource.Token);
                bool needUpdateEmailsFullListFile = await _ftpService.DownloadFullBaseIfNeededAsync(updateSource.Token);
                if (needUpdateEmailsFullListFile)
                {
                    while (!_allowedRecipients.isUpdateAllowed || updateSource.Token.IsCancellationRequested)
                    {
                    }
                    ReadRecipientsFromFile();
                }
            }
        }

        private void ReadRecipientsFromFile()
        {
            HashSet<string> addresses = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            try
            {
                using (FileStream fs = File.Open(recipientsFile, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (BufferedStream bs = new BufferedStream(fs))
                using (StreamReader sr = new StreamReader(bs))
                {
                    string? line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        addresses.Add(line.Trim());
                    }
                }
            }
            catch (Exception e)
            {
                WriteErrorAndExit(e.Message);
            }
            _allowedRecipients.UpdateRecipients(addresses);
        }
        private void WriteErrorAndExit(string message)
        {
            updateSource.Cancel();
            Console.Error.WriteLine("* Error: The address files not found");
            if (message != "") 
                Console.Error.WriteLine($"* {message}");
            Environment.Exit(1);
        }
    }

}
