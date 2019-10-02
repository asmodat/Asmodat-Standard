using AsmodatStandard.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using AsmodatStandard.Extensions.Collections;
using AsmodatStandard.Extensions.Threading;
using AsmodatStandard.Threading;
using System.Diagnostics;

namespace AsmodatStandard.IO
{
    public static class CLIHelper
    {
        public static Dictionary<string, string> GetNamedArguments(this string[] args, bool allowDuplicateArguments = true)
        {
            var namedArgs = new Dictionary<string, string>();
            foreach (var arg in args)
            {
                string value = null;
                string key;
                if (arg.Contains('='))
                {
                    var kv = arg.SplitByFirst('=');
                    key = kv[0].Trim(' ', '-', '=');
                    value = kv[1].Trim();

                    if ((value.StartsWith("'") && value.EndsWith("'")) ||
                        value.StartsWith("\"") && value.EndsWith("\""))
                        value = value.Substring(1, value.Length - 2); //remove quotes if present
                }
                else if (arg.StartsWith("-"))
                {
                    key = arg.Trim(' ', '-', '=');

                    if (key.Length <= 0)
                        continue; //empty strings can't be keys
                }
                else
                    continue;

                if (key.ContainsAny("[", "]"))
                    throw new Exception($"Key can't contain ']' or '[' characters, but was: {key}");

                if (namedArgs.ContainsKey(key) || namedArgs.ContainsKey($"{key}[]"))
                {
                    if (!allowDuplicateArguments)
                        throw new Exception($"Duplicate arguments are not allowed, but '{key}' is present more then once!");

                    var oldKey = key;
                    key = $"{key}[]";
                    if (namedArgs.ContainsKey(key) && !namedArgs[key].IsNullOrEmpty())
                        value = namedArgs[key].JsonDeserialize<string[]>().Merge(value).JsonSerialize();
                    else
                        value = (new string[] { namedArgs[oldKey], value }).JsonSerialize();

                    namedArgs.Remove(oldKey);
                }

                namedArgs[key] = value;
            }
            return namedArgs;
        }

        public static CommandOutput Bash(
            this string cmd,
            string workingDirectory,
            int timeout = 0)
        {
            cmd = cmd.Trim();
            bool isSudo = cmd.ToLower().StartsWith("sudo ");
            if (isSudo) // do not escape sudo
            {
                var args = cmd.SplitByFirst(' ')[1];
                return RunCommands.GetCommandOutputSimple(info: new ProcessStartInfo
                {
                    FileName = "sudo",
                    Arguments = args,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = workingDirectory
                }, timeout);
            }

            var escapedArgs = cmd.Replace("\"", "\\\"").Trim();

            var result = RunCommands.GetCommandOutputSimple(info: new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = $"-c \"{escapedArgs}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = workingDirectory
            }, timeout);

            return result;

        }

        public static CommandOutput CMD(
            this string cmd, 
            string workingDirectory,
            int timeout = 0)
        {
            var escapedArgs = cmd.Replace("\"", "\\\"");

            var result = RunCommands.GetCommandOutputSimple(info:new ProcessStartInfo
                {
                    FileName = @"C:\Windows\System32\cmd.exe",
                    Arguments = $"/c {cmd}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = workingDirectory,
                    Verb = "runas",
                }, timeout);

            return result;
        }

        



        /// <summary>
        /// Executes console command for windows or linux
        /// </summary>
        public static CommandOutput Console(this string cmd, string workingDirectory, int timeout = 0)
        {
            if (RuntimeEx.IsWindows())
                return CMD(cmd, workingDirectory, timeout);

            return Bash(cmd, workingDirectory, timeout);
        }

        public static CommandOutput Command(string fileName, string args, string workingDirectory, int timeout = 0)
        {
            return RunCommands.GetCommandOutputSimple(info: new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = args,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = workingDirectory,
                Verb = RuntimeEx.IsWindows() ? "runas" : null,
            }, timeout);
        }

    }

}