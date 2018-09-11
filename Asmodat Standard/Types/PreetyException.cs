using AsmodatStandard.Extensions;
using AsmodatStandard.Extensions.Collections;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace AsmodatStandard.Types
{
    public class PreetyExceptionStackTrace
    {
        public string Name { get; set; }
        public int Line { get; set; }
        public int Column { get; set; }
        public string MethodName { get; set; }

        public override string ToString()
            => $"{Name} => {MethodName}() l:{Line} c:{Column}";
    }

    public class PreetyException
    {
        public PreetyException(Exception ex, int maxDepth = 1000, int stackTraceMaxDepth = 1000)
        {
            if (ex == null)
                throw new ArgumentNullException($"{nameof(PreetyException)} doesn't allow for {nameof(ex)} parameter to be null.");

            this.Message = ex.Message;
            this.Type = ex.GetType().FullName;

            if (ex?.Data?.Keys != null)
            {
                this.Data = new Dictionary<object, object>();
                foreach (var k in ex.Data.Keys)
                    if (k?.ToString()?.StartsWith("UserDefined_") == true && ex.Data[k] != null)
                        this.Data.Add(k, ex.Data[k]);

                if (this.Data.IsNullOrEmpty())
                    this.Data = null;
            }

            if (stackTraceMaxDepth > 0 && ex.StackTrace != null)
            {
                var stackList = new List<string>();
                var stack = new StackTrace(ex, true);

                for (int i = 0; i < stackTraceMaxDepth; i++)
                {
                    var frame = stack.GetFrame(i);

                    if (frame == null)
                        break;

                    var trace = new PreetyExceptionStackTrace()
                    {
                        Name = frame.GetFileName(),
                        Line = frame.GetFileLineNumber(),
                        Column = frame.GetFileColumnNumber(),
                        MethodName = frame.GetMethod()?.Name
                    };

                    if (trace.Name.IsNullOrEmpty() && trace.Line == 0 && trace.Column == 0 &&
                        trace.MethodName.EquailsAny(StringComparison.InvariantCultureIgnoreCase, 
                        "HandleNonSuccessAndDebuggerNotification", 
                        "MoveNext", 
                        "Throw", 
                        "Wait",
                        "GetResultCore",
                        "ThrowIfExceptional"))
                        continue;

                    stackList.Add(trace.ToString());
                }

                if (!stackList.IsNullOrEmpty())
                    StackTrace = stackList.ToArray();
            }

            if (maxDepth > 0)
            {
                if (ex.InnerException != null)
                    this.InnerException = new PreetyException(ex.InnerException, (maxDepth - 1));

                if (ex is AggregateException)
                {
                    var aex = ex as AggregateException;

                    if (aex?.InnerExceptions != null)
                    {
                        var list = new List<PreetyException>();

                        foreach (var ae in aex.InnerExceptions)
                            if (ae != null)
                                list.Add(new PreetyException(ae, (maxDepth - 1)));

                        this.InnerExceptions = list.ToArray();
                    }
                }
            }
        }

        public string[] StackTrace { get; set; }
        public string Type { get; set; }
        public string Message { get; set; }
        public Dictionary<object, object> Data { get; set; }
        public PreetyException InnerException { get; set; }
        public PreetyException[] InnerExceptions { get; set; }

        public new string ToString()
            => this.JsonSerialize(
                Newtonsoft.Json.Formatting.Indented, 
                Newtonsoft.Json.ReferenceLoopHandling.Ignore,
                Newtonsoft.Json.NullValueHandling.Ignore);

        public string ToString(Newtonsoft.Json.Formatting formatting)
            => this.JsonSerialize(
                formatting,
                Newtonsoft.Json.ReferenceLoopHandling.Ignore,
                Newtonsoft.Json.NullValueHandling.Ignore);
    }
}