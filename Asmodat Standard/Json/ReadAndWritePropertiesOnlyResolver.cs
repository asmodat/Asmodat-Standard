using System;
using System.Collections.Generic;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Linq;

namespace AsmodatStandard.Json
{
    public class ReadAndWritePropertiesOnlyResolver : DefaultContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            IList<JsonProperty> props = base.CreateProperties(type, memberSerialization);
            return props.Where(p => p.Readable && p.Writable).ToList();
        }

        public static JsonSerializerSettings DefaulJsonSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new ReadAndWritePropertiesOnlyResolver(),
            MissingMemberHandling = MissingMemberHandling.Ignore
        };
    }
}