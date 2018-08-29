using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using AsmodatStandard.Extensions;
using AsmodatStandard.Extensions.Collections;
using Renci.SshNet;

namespace AsmodatStandard.Networking.SSH
{
    public class SshCommandResult
    {
        public DateTime RequestTime;
        public DateTime ResponseTime;
        public string Request;
        public string Response;
        public string Error;
        public int ExitStatus;
    }

    public class SSHManaged
    {
        private SshClient _client;

        public SSHManaged(string host, string username, FileInfo key)
        {
            key.Refresh();

            if (!key.Exists)
                throw new ArgumentException($"Key '{key.FullName}' does not exist.");

            var ci = new ConnectionInfo(host, username,
                new PrivateKeyAuthenticationMethod(username, new PrivateKeyFile(key.FullName)));

            _client = new SshClient(ci);
        }

        public void Connect(int maxConnectionRetry = 5, int maxConnectionRetryDelay = 5000)
            => TryCatchEx.ActionRepeat(() =>
            {
                if (_client.IsConnected == false)
                    _client.Connect(); //only connect if not connected already

                if (!_client.IsConnected)
                    throw new Exception($"Could not connect to host.");

            }, maxRepeats: maxConnectionRetry, onErrorAwait_ms: maxConnectionRetryDelay);

        public SshCommandResult ExecuteCommand(string commandText, int commandTimeout_ms)
            => ExecuteCommands(new string[] { commandText }, commandTimeout_ms).Single();

        public SshCommandResult[] ExecuteCommands(IEnumerable<string> commands, int commandTimeout_ms, int maxConnectionRetry = 5, int maxConnectionRetryDelay = 5000)
        {
            try
            {
                this.Connect(maxConnectionRetry: maxConnectionRetry, maxConnectionRetryDelay: maxConnectionRetryDelay);

                if (!_client.IsConnected)
                    throw new Exception("Could not connect.");

                var list = new List<SshCommandResult>();

                foreach (var c in commands)
                {
                    using (var cmd = _client.CreateCommand(c, Encoding.UTF8))
                    {
                        cmd.CommandTimeout = TimeSpan.FromMilliseconds(commandTimeout_ms);
                        var sw = Stopwatch.StartNew();
                        var dtRequest = DateTime.UtcNow;
                        var e = cmd.Execute();
                        sw.Stop();

                        list.Add(new SshCommandResult()
                        {
                            RequestTime = dtRequest,
                            ResponseTime = dtRequest.AddTicks(sw.ElapsedTicks),
                            Request = c,
                            Response = cmd.Result,
                            ExitStatus = cmd.ExitStatus,
                            Error = cmd.Error
                        });

                        if (cmd.ExitStatus != 0)
                            throw new Exception($"SSH Command Failed With Exit Status: {cmd.ExitStatus}, Error: '{cmd.Error}', Result: '{list.JsonSerialize(Newtonsoft.Json.Formatting.Indented)}'.");
                    }
                }

                return list.ToArray();
            }
            finally
            {
                _client?.Disconnect();
            }
        }

        public SshCommandResult[] ExecuteShellCommands(IEnumerable<string> commands, int commandTimeout_ms, int maxConnectionRetry = 5, int maxConnectionRetryDelay = 5000)
        {
            var eosMessage = "<End-Of-Stream/>";
            Exception error = null;
            var dataList = new List<byte>();
            var responseList = new List<SshCommandResult>();

            void Stream_ErrorOccurred(object sender, Renci.SshNet.Common.ExceptionEventArgs e) => error = e.Exception;
            void Stream_DataReceived(object sender, Renci.SshNet.Common.ShellDataEventArgs e)
            {
                if (!e.Data.IsNullOrEmpty())
                    dataList.AddRange(e.Data);
            }

            try
            {
                this.Connect(maxConnectionRetry: maxConnectionRetry, maxConnectionRetryDelay: maxConnectionRetryDelay);
                var stream = _client.CreateShellStream("xterm", 80, 24, 800, 600, 1024*1024);
                Stopwatch sw;

                void WriteCommand(string command)
                {
                    stream.WriteLine(command);
                    while (error == null && stream.Length == 0)
                    {
                        if (sw.ElapsedMilliseconds > commandTimeout_ms)
                            throw new Exception($"Command '{command}' timed out during WRITE operation, elapsed: {sw.ElapsedMilliseconds}/{commandTimeout_ms} [ms].");
                        else
                            Thread.Sleep(500);
                    }
                }

                stream.DataReceived += Stream_DataReceived;
                stream.ErrorOccurred += Stream_ErrorOccurred;

                foreach (var command in commands)
                {
                    sw = Stopwatch.StartNew();
                    var dtRequest = DateTime.UtcNow;
                    WriteCommand(command);
                    WriteCommand($"echo '{eosMessage}'");

                    string result = null;
                    while (error == null && ((result = dataList.ToArray().ToString(Encoding.UTF8)).ContainsCount(eosMessage) < 2))
                    {
                        if (sw.ElapsedMilliseconds > commandTimeout_ms)
                            throw new Exception($"Command '{command}' timed out during WRITE operation, elapsed: {sw.ElapsedMilliseconds}/{commandTimeout_ms} [ms].");
                        else
                            Thread.Sleep(500);
                    }

                    responseList.Add(new SshCommandResult()
                    {
                        RequestTime = dtRequest,
                        ResponseTime = dtRequest.AddTicks(sw.ElapsedTicks),
                        Request = command,
                        Response = dataList.ToArray().ToString(Encoding.UTF8),
                        ExitStatus = error == null ? 0 : 1,
                        Error = error.JsonSerializeAsPrettyException()
                    });

                    if (error != null)
                        throw new Exception($"SSH Command Failed With Error: '{error.JsonSerializeAsPrettyException()}', Result: '{responseList.JsonSerialize(Newtonsoft.Json.Formatting.Indented)}'.");

                    dataList = new List<byte>();
                }

                return responseList.ToArray();
            }
            finally
            {
                _client?.Disconnect();
            }
        }
    }
}