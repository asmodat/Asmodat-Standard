using AsmodatStandard.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AsmodatStandard.IO
{
    public static class CLIHelper
    {
        public static Dictionary<string, string> GetNamedArguments(this string[] args)
        {
            var namedArgs = new Dictionary<string, string>();
            foreach (var arg in args)
            {
                if (arg.Contains('='))
                {
                    var kv = arg.SplitByFirst('=');
                    var key = kv[0].Trim(' ', '-', '=');
                    var value = kv[1].Trim();

                    if ((value.StartsWith("'") && value.EndsWith("'")) ||
                        value.StartsWith("\"") && value.EndsWith("\""))
                        value = value.Substring(1, value.Length - 2); //remove quotes if present

                    if (namedArgs.ContainsKey(key))
                        throw new Exception($"Multiple Named arguments are not suported by {nameof(GetNamedArguments)}, key: '{key}' already exists.");

                    namedArgs[key] = value;
                }
                else if(arg.StartsWith("-"))
                {
                    var key = arg.Trim(' ', '-', '=');

                    if (key.Length <= 0)
                        continue; //empty strings can't be keys

                    namedArgs[key] = null;
                }
            }
            return namedArgs;
        }
    }
}
