using CGProToCCAddressHelper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CGProToCCAddressHelper.Services
{
    internal class WorkerService
    {
        private AllowedRecipients _allowedRecipients;
        private readonly AppSettings _appSettings;
        public WorkerService(AppSettings appSettings, AllowedRecipients allowedRecipients) 
        {
            _allowedRecipients = allowedRecipients;
            _appSettings = appSettings;
        }
        public async Task Work()
        {
            while (true)
            {
                string? line = await Console.In.ReadLineAsync();
                if (line == null)
                {
                    break;
                }
                _ = Task.Run(() =>
                {
                    ProcessMessage(line);
                });
            }
        }
        public void Print(string message)
        {
            Console.WriteLine(message);
            Console.Out.Flush();
        }
        private void ProcessMessage(string input)
        {
            _allowedRecipients.DisableUpdates();
            string[] inputParts = input.Split();
            if (inputParts.Length == 0)
            {
                return;
            }
            string lineNumberStr = inputParts[0];
            if (!int.TryParse(lineNumberStr, out _))
            {
                return;
            }
            string command = inputParts[1].ToLower();
            switch (command)
            {
                case "quit":
                    Print($"{lineNumberStr} OK");
                    Environment.Exit(0);
                    break;
                case "intf":
                    Print($"{lineNumberStr} INTF 3");
                    break;
                case "file":
                    if (inputParts.Length != 3)
                    {
                        return;
                    }
                    string fileName = inputParts[2];
                    var file = Path.Combine(_appSettings.baseDir, fileName.Trim());
                    ParseFile(file, lineNumberStr);
                    break;
                default:
                    break;
            }
            _allowedRecipients.EnableUpdates();
        }
        private void ParseFile(string file, string lineNumberStr)
        {
            FileInfo fileInfo = new FileInfo(file);
            if (!fileInfo.Exists) 
            {
                Print($"{lineNumberStr} OK");
                Print($"* CGProToCCAddressHelper: unable to read file {file}");
                return;
            }
            using (FileStream fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (BufferedStream bs = new BufferedStream(fs))
            using (StreamReader sr = new StreamReader(bs))
            {
                string pattern = @".*<(.*)>";
                string? line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.StartsWith("R W "))
                    {
                        Match regexMatch = Regex.Match(line, pattern);
                        if (regexMatch.Success)
                        {
                            string recipient = regexMatch.Groups[1].Value;
                            if (_allowedRecipients.isAddressNotAllowed(recipient))
                            {
                                Print($"{lineNumberStr} ERROR \"You are not allowed to send this message\"");
                                Print($"* CGProToCCAddressHelper: message to {recipient} discarded.");
                                return;
                            }
                        }
                    }
                }

            }
            Print($"{lineNumberStr} OK");
        }
    }
}
