using System;
using System.Collections.Generic;
using System.Reflection;
using Dot.Dependency;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Linq;

namespace Dot.Serializing
{
    [Registration(LifeCycle = LifeCycle.Singelton, RegisterMode = RegisterMode.Self | RegisterMode.DefaultInterface, Name = "jsonnet")]
    public class JsonNetSerializer : IJsonSerializer
    {
        private static readonly JsonSerializerSettings SETTINGS;

        static JsonNetSerializer()
        {
            SETTINGS = new JsonSerializerSettings
            {
                Converters = new List<JsonConverter> { new IsoDateTimeConverter() },
                ContractResolver = new CustomContractResolver(),
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                DateFormatHandling = DateFormatHandling.MicrosoftDateFormat
            };
        }

        public string Serialize(object obj)
        {
            return obj == null ? null : JsonConvert.SerializeObject(obj, SETTINGS);
        }

        public object Deserialize(string value, Type type)
        {
            return JsonConvert.DeserializeObject(value, type, SETTINGS);
        }

        public T Deserialize<T>(string value) where T : class
        {
            return JsonConvert.DeserializeObject<T>(JObject.Parse(value).ToString(), SETTINGS);
        }
    }

    class CustomContractResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var jsonProperty = base.CreateProperty(member, memberSerialization);
            if (jsonProperty.Writable) 
                return jsonProperty;

            var property = member as PropertyInfo;
            if (property == null) 
                return jsonProperty;

            var hasPrivateSetter = property.GetSetMethod(true) != null;
            jsonProperty.Writable = hasPrivateSetter;
            return jsonProperty;
        }
    }
}