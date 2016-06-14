using System.Text;
using Dot.ServiceModel;
using Newtonsoft.Json;

namespace Dot.Dubbo.Registery.ZooKeeper
{
    public static class ServiceMetadataExtension
    {
        public static byte[] ToBytes(this ServiceMetadata metadata)
        {
            var json = JsonConvert.SerializeObject(metadata);
            return Encoding.UTF8.GetBytes(json);
        }

        public static ServiceMetadata ToMetadata(this byte[] bytes)
        {
            var json = Encoding.UTF8.GetString(bytes);
            return JsonConvert.DeserializeObject<ServiceMetadata>(json);
        }
    }
}