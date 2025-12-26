using CGProToCCAddressHelper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static CGProToCCAddressHelper.Utils.Utils;

namespace CGProToCCAddressHelper.Services
{
    internal class WorkerService
    {
        private EmailChecker _emailChecker;
        private readonly AppSettings _appSettings;
        public WorkerService(AppSettings appSettings, EmailChecker emailChecker) 
        {
            _emailChecker = emailChecker;
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
        public void PrintGoodMessage(string lineNumber)
        {
            Print($"{lineNumber} OK");
        }
        public void PrintBadMessage(string lineNumber)
        {
            Print($"{lineNumber} ERROR \"You are not allowed to send this message\"");
        }
        public void Print(string message)
        {
            Console.WriteLine(message);
            Console.Out.Flush();
        }
        private void ProcessMessage(string input)
        {
            _emailChecker.DisableUpdates();
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
            _emailChecker.EnableUpdates();
        }
        public bool EnsureFileExists(string file, string lineNumberStr)
        {
            FileInfo fileInfo = new(file);
            if (!fileInfo.Exists)
            {
                PrintGoodMessage(lineNumberStr);
                PrintLogMessage($"{file} file does not exists");
                return false;
            }
            return true;
        }
        private void ParseFile(string file, string lineNumberStr)
        {
            if (!EnsureFileExists(file, lineNumberStr)) return;
            try
            {
                using (FileStream fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (BufferedStream bs = new BufferedStream(fs))
                using (StreamReader sr = new StreamReader(bs))
                {
                    string? line;
                    bool isSenderReplyAllowed = false;
                    string thisEmailSender = "";
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line == "") break;
                        string? sender = GetSender(line);
                        if (sender != null)
                        {
                            thisEmailSender = sender;
                            isSenderReplyAllowed = _emailChecker.isSenderReplyAllowed(sender);
                        }
                        string? recipient = GetRecipient(line);
                        if (recipient != null) 
                        {
                            if (_emailChecker.isSenderReplyAllowed(recipient))
                            {
                                _emailChecker.AddReplyAllowedRecipient(thisEmailSender);
                            }
                            if (_emailChecker.isAdressMonitored(thisEmailSender) && _emailChecker.isAddressNotAllowed(recipient))
                            {
                                if (!isSenderReplyAllowed || !_emailChecker.isRecipientReplyAllowed(recipient))
                                {
                                    PrintBadMessage(lineNumberStr);
                                    PrintLogMessage($"message to {recipient} discarded.");
                                    return;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                PrintLogMessage($"cannot process message {file}");
                PrintLogMessage($"{e.Message}");
            }
            PrintGoodMessage(lineNumberStr);

        }
    }
}
