using AsmodatStandard.Extensions;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace AsmodatStandard.Cryptography.Bitcoin
{
    public class BIP44Path
    {
        public BIP44Path() { }

        public BIP44Path(uint[] path)
        {
            if ((path?.Length ?? 0) < 5)
                throw new System.ArgumentException($"Path must have at least 5 levels, but had {path.Length}. Expected: m/purpose'/coin_type'/account'/change /address_index, but got: {path?.JsonSerialize() ?? "undefined"}");

            this.purpose = path[0];
            this.coin_type = path[1];
            this.account = path[2];
            this.change = path[3];
            this.address_index = path[4];
        }

        public BIP44Path(uint purpose, uint coin_type, uint account, uint change, uint address_index)
        {
            this.purpose = purpose;
            this.coin_type = coin_type;
            this.account = account;
            this.change = change;
            this.address_index = address_index;
        }

        public BIP44Path(string path)
        {
            var origin = path;

            var numbers = new List<uint>();
            string number = "";
            foreach(var c in $"{path}_")
            {
                if (c.IsDigit())
                    number += c;
                else
                {
                    if(number.Length > 0)
                    {
                        numbers.Add(number.ToUInt32());
                        number = "";
                        continue;
                    }
                }
            }

            if (numbers.Count < 5)
                throw new System.Exception($"Path must have at least 5 levels, but had {numbers.Count}. Expected: m/purpose'/coin_type'/account'/change /address_index, but got: {origin}");

            this.purpose = numbers[0];
            this.coin_type = numbers[1];
            this.account = numbers[2];
            this.change = numbers[3];
            this.address_index = numbers[4];
        }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public uint purpose { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public uint coin_type { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public uint account { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public uint change { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public uint address_index { get; set; }

        public override string ToString() => $"m/{ToSlimString()}";
        public string ToSlimString() => $"{purpose}'/{coin_type}'/{account}'/{change}/{address_index}";
        public uint[] ToArray() => new uint[] { purpose, coin_type, account, change, address_index };

    }

}
