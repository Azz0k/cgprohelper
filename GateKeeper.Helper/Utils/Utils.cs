using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CGPGK.Utils
{
    internal static class Utils
    {
        private static string me = "GateKeeper";
        public static string? GetRecipient(string line)
        {
            string pattern = @".*<(.*)>";
            if (line.StartsWith("R W "))
            {
                Match regexMatch = Regex.Match(line, pattern);
                if (regexMatch.Success)
                {
                    string recipient = regexMatch.Groups[1].Value;
                    return recipient;
                }
            }
            return null;
        }
        public static string? GetSender(string line)
        {
            string pattern = @".*<(.*)>";
            if (line.StartsWith("P I "))
            {
                Match regexMatch = Regex.Match(line, pattern);
                if (regexMatch.Success)
                {
                    string sender = regexMatch.Groups[1].Value;
                    return sender;
                }
            }
            return null;
        }
        public static void PrintLogMessage(string message)
        {
            Print($"* {me} {message}");
        }
        public static void PrintGoodMessage(string lineNumber)
        {
            Print($"{lineNumber} OK");
        }
        public static void PrintBadMessage(string lineNumber)
        {
            Print($"{lineNumber} ERROR \"{me}: You are not allowed to send this message\"");
        }

        public static void Print(string message)
        {
            Console.WriteLine(message);
            Console.Out.Flush();
        }
        public static bool EnsureFileExists(string file)
        {
            FileInfo fileInfo = new(file);
            if (!fileInfo.Exists)
            {
                return false;
            }
            return true;
        }
    }
}
