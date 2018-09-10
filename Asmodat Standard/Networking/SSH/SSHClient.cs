using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
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

        public Encoding Encoding => _client?.ConnectionInfo?.Encoding;

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

        public SshCommandResult[] ExecuteCommands(
            IEnumerable<string> commands, 
            int commandTimeout_ms, 
            int maxConnectionRetry = 5, 
            int maxConnectionRetryDelay = 5000)
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commands"></param>
        /// <param name="commandTimeout_ms"></param>
        /// <param name="maxConnectionRetry"></param>
        /// <param name="maxConnectionRetryDelay"></param>
        /// <param name="outputStream"></param>
        /// <param name="failSequence">Throws is sequence occurs in the output</param>
        /// <param name="breakSequence">Breaks execution if sequence occurs</param>
        /// <param name="passSequence">Throws is sequence does not occur by the ond of execution of the last command (if there were any)</param>
        /// <returns></returns>
        public SshCommandResult[] ExecuteShellCommands(
            IEnumerable<string> commands, 
            int commandTimeout_ms, 
            int maxConnectionRetry = 5, 
            int maxConnectionRetryDelay = 5000,
            int checkDelay = 500,
            Stream outputStream = null)
        {
            Exception error = null;
            ShellStream stream;
            Stopwatch sw;
            var dataList = new List<byte>();
            var consoleBuffer = new List<byte>();
            var responseList = new List<SshCommandResult>();

            try
            {
                this.Connect(maxConnectionRetry: maxConnectionRetry, maxConnectionRetryDelay: maxConnectionRetryDelay);
                stream = _client.CreateShellStream("xterm", 80, 24, 800, 600, 1024*1024);
                var encoding = this.Encoding;

                void Stream_ErrorOccurred(object sender, Renci.SshNet.Common.ExceptionEventArgs e) => error = e.Exception;
                void Stream_DataReceived(object sender, Renci.SshNet.Common.ShellDataEventArgs e)
                {
                    if (!e.Data.IsNullOrEmpty())
                        dataList.AddRange(e.Data);

                    if (outputStream != null)
                        outputStream.Write(e.Data, 0, e.Data.Length);
                }

                void TimeoutCheck(string command, string operation)
                {
                    if (sw.ElapsedMilliseconds > commandTimeout_ms)
                        throw new Exception($"Command '{command}' timed out during WRITE operation, elapsed: {sw.ElapsedMilliseconds}/{commandTimeout_ms} [ms]. Result: '{responseList.JsonSerialize(Newtonsoft.Json.Formatting.Indented)}");
                }

                void WriteCommand(string command)
                {
                    stream.WriteLine(command);
                    while (error == null && stream.Length == 0)
                    {
                        TimeoutCheck(command, "WRITE");
                        Thread.Sleep(checkDelay);
                    }
                }

                stream.DataReceived += Stream_DataReceived;
                stream.ErrorOccurred += Stream_ErrorOccurred;
                var count = 0;

                var commandFailureSequence = Guid.NewGuid().ToString();
                var endOfCommand = Guid.NewGuid().ToString();

                Thread.Sleep(checkDelay);
                dataList = new List<byte>(); //cleanup after connection initiation

                foreach (var command in commands)
                {
                    sw = Stopwatch.StartNew();
                    var dtRequest = DateTime.UtcNow;
                    ++count;
                    var commandCheck = $"if [ $? -ne 0 ]; then for n in {{1..3}}; do echo {commandFailureSequence}; done ; else for n in {{1..3}}; do echo {endOfCommand}; done ; fi";
                    
                    WriteCommand(command);

                    while(dataList.Count <= 0)
                    {
                        TimeoutCheck(command, "PUSH");
                        Thread.Sleep(checkDelay);
                    }

                    Thread.Sleep(checkDelay);
                    WriteCommand(commandCheck);

                    string result = null;
                    while (
                        error == null && 
                        (result.Count(endOfCommand) < 3) && 
                        (result.Count(commandFailureSequence) < 3))
                    {
                        result = dataList.ToArray().ToString(encoding);
                        TimeoutCheck(command, "READ");
                        Thread.Sleep(checkDelay);
                    }

                    var response = dataList.ToArray().ToString(this.Encoding);

                    responseList.Add(new SshCommandResult()
                    {
                        RequestTime = dtRequest,
                        ResponseTime = dtRequest.AddTicks(sw.ElapsedTicks),
                        Request = command,
                        Response = response,
                        ExitStatus = error == null ? 0 : 1,
                        Error = error.JsonSerializeAsPrettyException()
                    });

                    var failSerquenceFound = result.Count(commandFailureSequence) >= 3;
                    if (error != null || failSerquenceFound)
                        throw new Exception($"SSH Command {count} Failed, Error Occured: {error != null}, Command Failed: {failSerquenceFound} [{result.Count(commandFailureSequence)}], Curent Command: '{command}', Error: '{error.JsonSerializeAsPrettyException()}', End Sequences: {result.Count(endOfCommand)}.");

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