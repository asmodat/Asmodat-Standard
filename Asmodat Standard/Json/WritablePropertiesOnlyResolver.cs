using System;
using System.Collections.Generic;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Linq;

namespace AsmodatStandard.Json
{
    public class WritablePropertiesOnlyResolver : DefaultContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            IList<JsonProperty> props = base.CreateProperties(type, memberSerialization);
            return props.Where(p => p.Writable && !p.Readable).ToList();
        }

        public static JsonSerializerSettings DefaulJsonSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new WritablePropertiesOnlyResolver(),
            MissingMemberHandling = MissingMemberHandling.Ignore
        };
    }
}