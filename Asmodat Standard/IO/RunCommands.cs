using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace AsmodatStandard.IO
{
    public static class RunCommands
    {
        #region Outputs Property
        
        private static object _outputsLockObject;
        private static object OutputsLockObject
        {
            get
            {
                if (_outputsLockObject == null)
                    Interlocked.CompareExchange(ref _outputsLockObject, new object(), null);
                return _outputsLockObject;
            }
        }

        private static ConcurrentDictionary<int, Dictionary<object, CommandOutput>> _outputs;
        private static ConcurrentDictionary<int, Dictionary<object, CommandOutput>> Outputs
        {
            get
            {
                if (_outputs != null)
                    return _outputs;

                lock (OutputsLockObject)
                {
                    _outputs = new ConcurrentDictionary<int, Dictionary<object, CommandOutput>>();
                }
                return _outputs;
            }
        }
        
        #endregion

        public static CommandOutput GetCommandOutputSimple(ProcessStartInfo info, int timeout)
        {
            // Redirect the output stream of the child process.
            info.UseShellExecute = false;
            info.CreateNoWindow = true;
            info.RedirectStandardOutput = true;
            info.RedirectStandardError = true;
            var process = new Process();
            process.StartInfo = info;
            process.ErrorDataReceived += ErrorDataHandler;
            process.OutputDataReceived += OutputDataHandler;

            var output = new CommandOutput();
            int pid;
            lock (OutputsLockObject)
            {
                process.Start();
                pid = process?.Id ?? 0;
                if (!Outputs.ContainsKey(pid))
                    Outputs[pid] = new Dictionary<object, CommandOutput>();
            }
            Outputs[pid].Add(process, output);

            process.BeginErrorReadLine();
            process.BeginOutputReadLine();

            if(timeout <= 0)
                process.WaitForExit();
            else
                process.WaitForExit(timeout);

            Outputs[pid].Remove(process);

            if (timeout > 0 && !process.HasExited)
                output.Error = $"{output?.Error ?? ""}Command timed out, passed maximum of {timeout}ms.";

            output.Error = output?.Error?.TrimEnd('\n');
            output.Output = output?.Output?.TrimEnd('\n');
            return output;
        }

        private static void ErrorDataHandler(object sendingProcess, DataReceivedEventArgs errLine)
        {
            var pid = sendingProcess != null && sendingProcess is Process ? ((Process)sendingProcess)?.Id ?? 0 : 0;

            if (errLine.Data == null)
                return;

            if (!Outputs[pid].ContainsKey(sendingProcess))
                return;

            var commandOutput = Outputs[pid][sendingProcess];

            commandOutput.Error = (commandOutput?.Error ?? "") + errLine.Data + "\n";
        }

        private static void OutputDataHandler(object sendingProcess, DataReceivedEventArgs outputLine)
        {
            var pid = sendingProcess != null && sendingProcess is Process ? ((Process)sendingProcess)?.Id ?? 0 : 0;

            if (outputLine.Data == null)
                return;

            if (!Outputs[pid].ContainsKey(sendingProcess))
                return;

            var commandOutput = Outputs[pid][sendingProcess];

            commandOutput.Output = (commandOutput?.Output ?? "") + outputLine.Data + "\n";
        }
    }
    public class CommandOutput
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Error { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Output { get; set; }
    }
}
