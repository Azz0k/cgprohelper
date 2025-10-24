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
            try
            {
                await GetDataFromFTP();
            }
            catch (Exception ex)
            {
                WriteErrorAndExit(ex.Message);
            }
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
                while (!_allowedRecipients.isUpdateAllowed || updateSource.Token.IsCancellationRequested)
                {
                }
                await GetDataFromFTP();
            }
        }

        private async Task GetDataFromFTP()
        {
            List<UpdatesFromFile> updates = await _ftpService.DownloadIfNeededAsync(updateSource.Token);
            foreach (UpdatesFromFile update in updates)
            {
                switch (update.FileType)
                {
                    case FileTypes.EmailsFull:
                        _allowedRecipients.UpdateRecipients(update.Data); break;
                    case FileTypes.EmailsDiff:
                        _allowedRecipients.AddRecipients(update.Data); break;
                    case FileTypes.DomainsFull:
                    case FileTypes.DomainsDiff:
                        _allowedRecipients.AddDomains(update.Data); break;  
                        default:break;
                }
            }
        }
        private void WriteErrorAndExit(string message="")
        {
            updateSource.Cancel();
            Console.Error.WriteLine("* Error: The address files not found");
            if (message != "") 
                Console.Error.WriteLine($"* {message}");
            Environment.Exit(1);
        }
    }

}
