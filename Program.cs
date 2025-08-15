using System.Linq;
using System.Text.RegularExpressions;

namespace CGProToCCAddressHelper
{
    internal class Program
    {
        static HashSet<string> allowedRecipients = new HashSet<string>();
        static string baseDir = "";
        static private string[] domains = {"@dcenergo.ru", "@tdenergospb.ru", "@energospb.ru", "@bcenergo.ru", "@dsolutions.tech", "@energo.am", "@frogsup.ru", "@helptomama.ru" };
        static HashSet<string> allowedDomains = new HashSet<string>(domains);
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            string currentDir = Directory.GetCurrentDirectory();
            string recipientsFile = Path.Combine(currentDir, "allowedRecipients.csv");
            string baseIniFile = Path.Combine(currentDir, "baseDir.ini");
            Print("* ToCCAddressHelper Free");
            try
            {
                baseDir = File.ReadAllText(baseIniFile).Trim();
                using (FileStream fs = File.Open(recipientsFile, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (BufferedStream bs = new BufferedStream(fs))
                using (StreamReader sr = new StreamReader(bs))
                {
                    string? line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        allowedRecipients.Add(line.Trim());
                    }

                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("* Error address files not found");
                Console.Error.WriteLine(e.Message);
                Environment.Exit(1);
            }
            while (true)
            {
                string? line = await Console.In.ReadLineAsync();
                if (line == null)
                {
                    break;
                }
                _ = System.Threading.Tasks.Task.Run(() =>
                {
                    ProcessMessage(line);
                });
            }


        }
        static void ProcessMessage(string input)
        {
            string[] inputParts = input.Split();
            if (inputParts.Length == 0)
            {
                return;
            }
            string lineNumberStr = inputParts[0];
            if (!Int32.TryParse(lineNumberStr, out _))
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
                    var file = Path.Combine(baseDir, fileName.Trim());
                    using (FileStream fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read))
                    using (BufferedStream bs = new BufferedStream(fs))
                    using (StreamReader sr = new StreamReader(bs))
                    {
                        string pattern = @".*<(.*)>";
                        string? line;
                        while ((line = sr.ReadLine()) != null)
                        {
                            if (line.Trim() == "")
                            {
                                break;
                            }
                            if (line.StartsWith("R W "))
                            {
                                Match regexMatch = Regex.Match(line, pattern);
                                if (regexMatch.Success)
                                {
                                    string recipient = regexMatch.Groups[1].Value;
                                    string domain = recipient.Substring(recipient.IndexOf('@'));
                                    if (!allowedDomains.Contains(domain) && !allowedRecipients.Contains(recipient))
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
                    break;
                default:
                    break;
            }
            
        }
        static void Print(string message)
        {
            Console.WriteLine(message);
            Console.Out.Flush();
        }
    }
}
