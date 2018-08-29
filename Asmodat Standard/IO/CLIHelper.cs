using AsmodatStandard.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using AsmodatStandard.Extensions.Collections;

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
    }
}
